using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FirstInFirstAid.DAL;
using FirstInFirstAid.Models;
using log4net;
using System.Reflection;
using System;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Configuration;

namespace FirstInFirstAid.Controllers
{
    [Authorize]
    public class EventSegmentsController : Controller
    {
        private FirstInFirstAidDBContext db = new FirstInFirstAidDBContext();
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string senderEmailAddress = ConfigurationManager.AppSettings["SenderEmailAddress"];
        private static readonly string senderName = ConfigurationManager.AppSettings["SenderName"];

        // GET: EventSegments
        public ActionResult Index()
        {
            var eventSegments = db.EventSegments.Include(c => c.ClientContact).Include(e => e.Event).Include(v => v.Venue);
            return View(eventSegments.ToList());
        }

        // POST: EventSegments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult Create(EventSegment eventSegment, int eventId)
        {
            if (ModelState.IsValid)
            {
                Event evnt = db.Events.Include(c => c.EventSegments).Where(i => i.Id == eventId).First();
                eventSegment.Hours = (eventSegment.EndTime - eventSegment.StartTime).Hours;
                evnt.EventSegments.Add(eventSegment);
                db.SaveChanges();
                logger.InfoFormat("Event Segment Created, Name : {0}, Id: {1}", eventSegment.Name, eventSegment.Id);
                return Json(new
                {
                    Name = eventSegment.Name.ToString(),
                    StartTime = eventSegment.StartTime.ToString(),
                    EndTime = eventSegment.EndTime.ToString(),
                    Hours = eventSegment.Hours,
                    RequiredNumberOfStaff = eventSegment.RequiredNumberOfStaff,
                    Id = eventSegment.Id
                });
            }
            return Json("Invalid Model State");
        }

        // GET: EventSegments/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                logger.Warn("Received null Event Segment Id to modify");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventSegment eventSegment = db.EventSegments.Include(e => e.Event).
                Include(c => c.ClientContact).
                Include(v => v.Venue).
                Include(a => a.TrainorAllocations.Select(t => t.Trainor)).
                Where(x => x.Id == id).First();

            EventSegment eventSegment1 = db.EventSegments.Include("TrainorAllocations.Trainor").
                Where(x => x.Id == id).First();
            if (eventSegment == null)
            {
                logger.Warn("Received null Event Segement Id to modify");
                return HttpNotFound();
            }

            //Select List for duty type
            var dutyType = from DutyType d in Enum.GetValues(typeof(DutyType))
                           select new { ID = (int)d, Name = d.ToString() };
            ViewBag.DutyTypeEnum = JsonConvert.SerializeObject(dutyType);
            ViewBag.DutyTypeSelectList = dutyType.ToList();

            //Select List for trainers
            var trainerList = from Trainor t in getAvailableTrainersList(id) select new { ID = t.Id, Name = t.FirstName + " " + t.Lastname };
            ViewBag.Trainers = trainerList;

            //Select List for venues
            var venuelist = from Venue v in db.Venues select new { ID = v.Id, Name = v.VenueName };
            ViewBag.Venues = new SelectList(venuelist, "ID", "Name", eventSegment.Venue?.Id);

            //Select List for Client Contacts
            Event evnt = db.Events.Include(x => x.Client).Where(z => z.Id == eventSegment.Event.Id).First();
            if (evnt.Client != null)
            {
                var clientContactList = from ClientContact cc in db.ClientContacts.Where(x => x.Client.Id == evnt.Client.Id)
                                        select new { ID = cc.Id, Name = cc.ContactName };
                ViewBag.ClientContact = new SelectList(clientContactList, "ID", "Name", eventSegment.ClientContact?.Id);
            }
            else
            {
                ViewBag.ClientContact = Enumerable.Empty<SelectListItem>();
            }

            //select list for true false values
            var list = new[] { new { Text = "True", Value = "true" }, new { Text = "False", Value = "false" } };
            ViewBag.BoolList = new SelectList(list, "Value", "Text");

            return View(eventSegment);
        }

        // POST: EventSegments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,StartTime,EndTime,Hours,RequiredNumberOfstaff")] EventSegment eventSegment,
            int? venueId, int? clientContactId)
        {
            if (ModelState.IsValid)
            {
                logger.DebugFormat("Modifying Event Segment of the Name: {0} and Id:{}", eventSegment.Name, eventSegment.Id);
                EventSegment dbEventSegment = db.EventSegments.Include(v => v.Venue).Include(c => c.ClientContact).Where(i => i.Id == eventSegment.Id).First();

                //Updating the Event Segment fields
                dbEventSegment.Name = eventSegment.Name;
                dbEventSegment.Hours = (eventSegment.EndTime - eventSegment.StartTime).Hours;
                dbEventSegment.StartTime = eventSegment.StartTime;
                dbEventSegment.EndTime = eventSegment.EndTime;
                dbEventSegment.RequiredNumberOfStaff = eventSegment.RequiredNumberOfStaff;

                //Updating the Client Contact
                if (clientContactId != null) { 
                    ClientContact existingClientContact = dbEventSegment.ClientContact;
                    if (existingClientContact != null)
                    {
                        if (clientContactId > 0 && clientContactId != existingClientContact.Id) // A new client contact is assigned
                        {
                            ClientContact newClientContact = db.ClientContacts.Include(e => e.EventSegments).Where(i => i.Id == clientContactId).First();
                            existingClientContact.EventSegments.Remove(dbEventSegment);
                            newClientContact.EventSegments.Add(dbEventSegment);
                        }
                    }
                    else  //When the event segment is not assigned a client contact before
                    {
                        ClientContact newClientContact = db.ClientContacts.Include(e => e.EventSegments).Where(i => i.Id == clientContactId).First();
                        newClientContact.EventSegments.Add(dbEventSegment);
                    }
                }

                //Updating the Venue
                if (venueId != null)
                {
                    Venue existingVenue = dbEventSegment.Venue;
                    if (existingVenue != null)
                    {
                        if (venueId > 0 && venueId != existingVenue.Id) // A new venue is assigned
                        {
                            Venue newVenue = db.Venues.Include(e => e.EventSegments).Where(i => i.Id == venueId).First();
                            existingVenue.EventSegments.Remove(dbEventSegment);
                            newVenue.EventSegments.Add(dbEventSegment);
                        }
                    }

                    else if (venueId > 0) //When the event segment is not assigned a venue before
                    {
                        Venue newVenue = db.Venues.Include(e => e.EventSegments).Where(i => i.Id == venueId).First();
                        newVenue.EventSegments.Add(dbEventSegment);
                    }
                }

                db.SaveChanges();
                logger.InfoFormat("Event Segment modified successfully, Name: {0}, Id: {1}", eventSegment.Name, eventSegment.Id);
                return RedirectToAction("Index");
            }

            return View(eventSegment);
        }

        // GET: EventSegments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                logger.Warn("Received null Event Segment Id to delete");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventSegment eventSegment = db.EventSegments.Find(id);
            if (eventSegment == null)
            {
                logger.WarnFormat("Event Segment not found to delete, Id: {0}", id);
                return HttpNotFound();
            }
            return View(eventSegment);
        }

        // POST: EventSegments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EventSegment eventSegment = db.EventSegments.Find(id);
            db.EventSegments.Remove(eventSegment);
            db.SaveChanges();
            logger.InfoFormat("Event Segment deleted, Name: {0}, Id: {1}", eventSegment.Name, eventSegment.Id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> SendMail(int trainerId, string subject, string messageBody)
        {
            Trainor trainer = db.Trainors.Find(trainerId);
            if (trainer != null && trainer.Email != null) {
                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress(trainer.Email));
                message.Subject = subject;
                message.Body = string.Format(body, senderName, senderEmailAddress, messageBody);
                message.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                    var successMessage = "Email Successfully sent to " + trainer.FirstName;
                    logger.InfoFormat("Email Successfully sent to {0} {1}", trainer.FirstName, trainer.Lastname);
                    return Content("{\"Type\":\"Success\", \"Message\":\"" + successMessage + "\"}");
                }
            }
            var errorMessage = (trainer == null ? "Specified trainer was not found" : "Specified trainer doesn't have an assigned email address");
            return Content("{\"Type\":\"Warn\", \"Message\":\"" + errorMessage + "\"}");

        }

        public JsonResult getAvailableTrainers(int segmentId)
        {
            return Json(getAvailableTrainersList(segmentId), JsonRequestBehavior.AllowGet);
        }

        public List<Trainor> getAvailableTrainersList(int segmentId)
        {
            EventSegment segment = db.EventSegments.Include(seg => seg.TrainorAllocations.Select(allo => allo.Trainor)).Where(s => s.Id == segmentId).First();

            List<EventSegment> overlappingSegs = db.EventSegments.Include(seg => seg.TrainorAllocations.Select(allo => allo.Trainor)).
                    Where(s => s.EndTime >= segment.StartTime && s.StartTime <= segment.EndTime).ToList();

            overlappingSegs.Add(segment);

            List<Trainor> allocatedTrainers = new List<Trainor>();
            foreach (var eventSegment in overlappingSegs)
            {
                foreach (var allocation in eventSegment.TrainorAllocations)
                {
                    allocatedTrainers.Add(allocation.Trainor);
                }
            }

            return db.Trainors.ToList().Except(allocatedTrainers).ToList();
        }

        public JsonResult getClientContacts(int eventId)
        {
            Event evnt = db.Events.Include(x => x.Client).Where(z => z.Id == eventId).First();
            if (evnt.Client != null)
            {
                return Json(db.ClientContacts.Where(x => x.Client.Id == evnt.Client.Id).Select(i => new { i.ContactName, i.Id }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("A client Is not assigned to the event");
            }
        }

        public JsonResult getVenues()
        {
            return Json(db.Venues.Select(i => new { i.Id, i.VenueName }), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getupComingEventSegments() {
            DateTime sevenDays = DateTime.Today.AddDays(7);
            var upcoming = from EventSegment e in db.EventSegments.Where(s => s.StartTime < sevenDays) select new { Name = e.Name, Start = e.StartTime, Venue = e.Venue.VenueName };
            var json = new { data = upcoming };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
