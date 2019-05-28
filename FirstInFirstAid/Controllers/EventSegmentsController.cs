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
using Newtonsoft.Json.Converters;

namespace FirstInFirstAid.Controllers
{
    [Authorize]
    public class EventSegmentsController : Controller
    {
        private FirstInFirstAidDBContext db = new FirstInFirstAidDBContext();
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string senderEmailAddress = ConfigurationManager.AppSettings["SenderEmailAddress"];
        private static readonly string senderName = ConfigurationManager.AppSettings["SenderName"];
        private const string _dateFormat = "yyyy-MM-dd HH:mm:ss";
        private IsoDateTimeConverter isoConvert = new IsoDateTimeConverter();
        
        public EventSegmentsController()
        {
            isoConvert.DateTimeFormat = _dateFormat;
        }

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
            if (ModelState.ContainsKey("eventSegment.Event"))
            {
                ModelState["eventSegment.Event"].Errors.Clear();
            }
            if (eventSegment.StartTime > eventSegment.EndTime)
            {
                ModelState.AddModelError("EndTime", "End Time should be greater than Start Time");
                return Json(new
                    { Type = "EndTime" }
                );
            }
            if (ModelState.IsValid)
            {
                Event evnt = db.Events.Include(c => c.EventSegments).Where(i => i.Id == eventId).First();                
                eventSegment.Hours = eventSegment.EndTime.Subtract(eventSegment.StartTime).TotalHours;
                eventSegment.TotalFee = evnt.HourlyRate * eventSegment.RequiredNumberOfStaff * eventSegment.Hours;
                evnt.EventSegments.Add(eventSegment);
                db.SaveChanges();
                logger.InfoFormat("Event Segment Created, Name : {0}, Id: {1}", eventSegment.Name, eventSegment.Id);
                return Json(new
                {
                    Type = "Success",
                    Message = "Successfully created Event Segment, " + eventSegment.Name
                });
            }
            return Json(new
            {
                Type = "Error",
                Message = "Error while creating Event Eegment " +
                     string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))
            });
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

            //db.Entry(eventSegment?.Venue).Reference(v => v.Address).Load();

           

            if (eventSegment == null)
            {
                logger.Warn("Received null Event Segement Id to modify");
                return HttpNotFound();
            }
            GenerateViewBagItems(eventSegment);
            return View(eventSegment);
        }

        // POST: EventSegments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,StartTime,EndTime,Hours,RequiredNumberOfstaff,Coverage")] EventSegment eventSegment,
            int? venueId, int? clientContactId)
        {
            if (ModelState.ContainsKey("Event"))
            {
                ModelState["Event"].Errors.Clear();
            }
            if (eventSegment.StartTime > eventSegment.EndTime)
            {
                ModelState.AddModelError("EndTime", "End Time should be greater than Start Time");
            }

            if (ModelState.IsValid)
            {
                logger.DebugFormat("Modifying Event Segment of the Name: {0} and Id:{}", eventSegment.Name, eventSegment.Id);
                EventSegment dbEventSegment = db.EventSegments.Include(e => e.Event).Include(v => v.Venue).Include(c => c.ClientContact).Where(i => i.Id == eventSegment.Id).First();

                //Updating the Event Segment fields
                dbEventSegment.Name = eventSegment.Name;
                dbEventSegment.Hours = eventSegment.EndTime.Subtract(eventSegment.StartTime).TotalHours;
                dbEventSegment.StartTime = eventSegment.StartTime;
                dbEventSegment.EndTime = eventSegment.EndTime;
                dbEventSegment.RequiredNumberOfStaff = eventSegment.RequiredNumberOfStaff;
                dbEventSegment.Coverage = eventSegment.Coverage;
                dbEventSegment.TotalFee = dbEventSegment.Event.HourlyRate * dbEventSegment.RequiredNumberOfStaff * dbEventSegment.Hours;
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
            GenerateViewBagItems(eventSegment);
            return View(eventSegment);
        }

        private void GenerateViewBagItems(EventSegment eventSegment)
        {
            if (eventSegment.Event == null)
            {
                eventSegment.Event = db.EventSegments.Include(e => e.Event).First().Event;
            }
            //Select List for duty type
            var dutyType = from Coverage d in Enum.GetValues(typeof(Coverage))
                           select new { ID = (int)d, Name = d.ToString() };
            ViewBag.DutyTypeEnum = JsonConvert.SerializeObject(dutyType);
            ViewBag.DutyTypeSelectList = dutyType.ToList();

            //Select List for trainers
            var trainerList = from Trainor t in getAvailableTrainersList(eventSegment.Id) select new { ID = t.Id, Name = t.FirstName + " " + t.Lastname };
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
            var list = new[] { new { Text = "Yes", Value = "true" }, new { Text = "No", Value = "false" } };
            ViewBag.BoolList = new SelectList(list, "Value", "Text");
        }

        // GET: EventSegments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                logger.Warn("Received null Event Id to delete");

                return Content("{\"Type\":\"Warn\", \"Message\":\"Received null Event Segment Id to delete\"}");

            }
            else
            {

                EventSegment eventSegment = db.EventSegments.Find(id);
                if (eventSegment == null)
                {
                    logger.WarnFormat("Event Segment not found to delete, Id: {0}", id);
                    return Content("{\"Type\":\"Warn\", \"Message\":\"Event Segment not found to delete with the Id:" + id + "\"}");
                }
                else
                {
                    return Content("{\"Type\":\"Confirm\", \"Message\":\"Are you sure you want to delete the Event segment: " + eventSegment.Name + "\", \"Id\": \" " + eventSegment.Id + "\"}");
                }
            }
        }

        // POST: EventSegments/Delete/5
        [HttpPost]
        public JsonResult DeleteConfirmed(int id)
        {
            EventSegment eventSegment = db.EventSegments.Find(id);
            db.EventSegments.Remove(eventSegment);
            db.SaveChanges();
            logger.InfoFormat("Event Segment deleted, Name: {0}, Id: {1}", eventSegment.Name, eventSegment.Id);
            return Json("Successfully deleted the Event: " + eventSegment.Name);
        }

        [HttpPost]
        public async Task<ActionResult> SendMail(int id, string subject, string messageBody, string idName)
        {
            string email;
            string name;
            if ("trainerId".Equals(idName))
            {
                Trainor trainer = db.Trainors.Find(id);
                if (trainer != null && trainer.Email != null)
                {
                    email = trainer.Email;
                    name = trainer.FirstName + " " + trainer.Lastname;
                }
                else
                {
                    var errorMessage = (trainer == null ? "Specified trainer was not found" : "Specified trainer doesn't have an email address assigned");
                    return Content("{\"Type\":\"Warn\", \"Message\":\"" + errorMessage + "\"}");
                }
            } else
            {
                ClientContact clientContact = db.ClientContacts.Find(id);
                if (clientContact != null && clientContact.ContactEmail != null)
                {
                    email = clientContact.ContactEmail;
                    name = clientContact.ContactName;
                } else
                {
                    var errorMessage = (clientContact == null ? "Specified client contact was not found" : "Specified client contact doesn't have an email address assigned");
                    return Content("{\"Type\":\"Warn\", \"Message\":\"" + errorMessage + "\"}");
                }
            }
            messageBody.Replace("\n", "<br/>");
            System.Web.HttpUtility.HtmlEncode(messageBody);
            var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
            var message = new MailMessage();
            message.To.Add(new MailAddress(email));
            message.Subject = subject;
            message.Body = string.Format(body, senderName, senderEmailAddress, messageBody);
            message.IsBodyHtml = true;
            using (var smtp = new SmtpClient())
            {
                try
                {
                    await smtp.SendMailAsync(message);
                    var successMessage = "Email Successfully sent to " + name;
                    logger.InfoFormat("Email Successfully sent to {0}", name);
                    return Content("{\"Type\":\"Success\", \"Message\":\"" + successMessage + "\"}");
                }
                catch(Exception e)
                {
                    return Content("{\"Type\":\"Success\", \"Message\":\"" + e.Message + "\"}");
                }
            }             
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

        public JsonResult getEmailBodyTrainer(int id)
        {
            EventSegment eventSegment = db.EventSegments.Include(e => e.Event).
               Include("Event.Client").
               Include(c => c.ClientContact).
               Include(v => v.Venue).
               Include("Venue.Address").
               Where(x => x.Id == id).First();

            string mailBody = "Are you available for the following event? \n\nEvent Name : " + eventSegment.Event.EventName + "\n"
                + "Event Segment Name : " + eventSegment.Name + "\n"
                + "Venue : " + eventSegment.Venue?.Address.ToString() + "\n"
                + "Start Time : " + eventSegment.StartTime + "\n"
                + "End Time : " + eventSegment.EndTime + "\n"
                + "Number of Hours :" + eventSegment.Hours + "\n"
                + "Duty Type : " + eventSegment.Coverage.ToString() + "\n";

            if (eventSegment.Event.Client != null)
            {
                mailBody = mailBody
                    + "Client : " + eventSegment.Event.Client.Name + "\n";                       
            }

            if (eventSegment.ClientContact != null)
            {
                mailBody = mailBody +
                    "Client Contact: " + eventSegment.ClientContact.ContactName + ", Ph: " + eventSegment.ClientContact.ContactPhone +
                        ", OfficePh: " + eventSegment.ClientContact.ContactPhoneOff;
            }
            return Json (new { Body = mailBody, Subject = eventSegment.Coverage.ToString() }, JsonRequestBehavior.AllowGet);   
        }

        public JsonResult getEmailBodyClient(int id)
        {
            EventSegment eventSegment = db.EventSegments.Include(e => e.Event).
               Include("Event.Client").
               Include(c => c.ClientContact).
               Include(v => v.Venue).
               Include("Venue.Address").
               Where(x => x.Id == id).First();

            string mailBody =
                "Event Name: " + eventSegment.Event.EventName + "\n"
                + "Event Segment Name : " + eventSegment.Name + "\n"
                + "Venue : " + eventSegment.Venue?.Address.ToString() + "\n"
                + "Start Time : " + eventSegment.StartTime + "\n"
                + "End Time : " + eventSegment.EndTime + "\n"
                + "Number of Hours :" + eventSegment.Hours + "\n"
                + "Duty Type : " + eventSegment.Coverage.ToString() + "\n"
                + "Total Fee : " + eventSegment.TotalFee + "\n"
                + "Number of Staff : " + eventSegment.RequiredNumberOfStaff + "\n";

            return Json(new { Body = mailBody, Subject = eventSegment.Coverage.ToString() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getupComingEventSegments() {
            List<object> list = new List<object>();
            var upcoming = from EventSegment e in db.EventSegments.Where(s => s.StartTime > DateTime.Today) select new { Name = e.Name, Start = e.StartTime, Venue = e.Venue.VenueName, EventName = e.Event.EventName };
           
            foreach (var segment in upcoming.ToList()) {
                String date = JsonConvert.SerializeObject(segment.Start, isoConvert);
                String correctDate = date.Substring(1, date.Length - 2);
                list.Add(new {Name=segment.Name, Start = correctDate, Venue = segment.Venue, EventName = segment.EventName });
            }
            var json = new { data = list };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getDutyTypeState()
        {
            List<object> list = new List<object>();
            foreach (EventSegment segment in db.EventSegments.Include(x => x.TrainorAllocations))
            {
                if (segment.RequiredNumberOfStaff > segment.TrainorAllocations.Count())
                {
                    list.Add(new {ID=segment.Id, State="incomplete" });
                }
                else
                {
                    Boolean allConfirmed = true;
                    foreach (TrainorAllocationForEventSeg allocation in segment.TrainorAllocations)
                    {
                        if (!allocation.PresenceConfirmation)
                        {
                            allConfirmed = false;
                        }
                    }
                    
                    list.Add(new { ID = segment.Id, State = "complete", Confirmed  = allConfirmed, DutyType = (Coverage)segment.Coverage });
                }
            }
          
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult needMoreTrainerAllocs(int id)
        {
            EventSegment eventSegment = db.EventSegments.Include(e => e.TrainorAllocations).     
              Where(x => x.Id == id).First();

            Boolean needMoreAllocs;
            if (eventSegment.RequiredNumberOfStaff > eventSegment.TrainorAllocations.Count())
            {
                needMoreAllocs = true;
            } 
            else
            {
                needMoreAllocs = false;
            }

            return Json(needMoreAllocs, JsonRequestBehavior.AllowGet);
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
