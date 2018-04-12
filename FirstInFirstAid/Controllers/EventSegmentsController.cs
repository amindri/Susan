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

namespace FirstInFirstAid.Controllers
{
    public class EventSegmentsController : Controller
    {
        private FirstInFirstAidDBContext db = new FirstInFirstAidDBContext();
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        // GET: EventSegments
        public ActionResult Index()
        {
            var eventSegments = db.EventSegments;
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
            //IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
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
            EventSegment eventSegment = db.EventSegments.Include(e => e.Event).Include(c => c.ClientContact).Include(v => v.Venue).Where(x => x.Id == id).First();
            if (eventSegment == null)
            {
                logger.Warn("Received null Event Segement Id to modify");
                return HttpNotFound();
            }

            //Select List for duty type
            var dutyType = from DutyType d in Enum.GetValues(typeof(DutyType))
                           select new { ID = (int)d, Name = d.ToString() };
            ViewBag.DutyTypeEnum = JsonConvert.SerializeObject(new SelectList(dutyType, "ID", "Name"));                     

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

            return View(eventSegment);
        }

        // POST: EventSegments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,StartTime,EndTime,Hours,RequiredNumberOfstaff")] EventSegment eventSegment, 
            int venueId, int clientContactId)
        {
            if (ModelState.IsValid)
            {
                logger.DebugFormat("Modifying Event Segment of the Name: {0} and Id:{}", eventSegment.Name, eventSegment.Id);
                EventSegment dbEventSegment = db.EventSegments.Include(v => v.Venue).Include(c => c.ClientContact).Where(i => i.Id == eventSegment.Id).First();

                //Updating the Event Segment fields
                dbEventSegment.Name = eventSegment.Name;
                dbEventSegment.Hours = eventSegment.Hours;
                dbEventSegment.StartTime = eventSegment.StartTime;
                dbEventSegment.EndTime = eventSegment.EndTime;
                dbEventSegment.RequiredNumberOfStaff = eventSegment.RequiredNumberOfStaff;

                //Updating the Client Contact
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
                else if (clientContactId > 0) //When the event segment is not assigned a client contact before
                {
                    ClientContact newClientContact = db.ClientContacts.Include(e => e.EventSegments).Where(i => i.Id == clientContactId).First();
                    newClientContact.EventSegments.Add(dbEventSegment);
                }

                //Updating the Venue
                Venue existingVenue = dbEventSegment.Venue;
                if (existingVenue != null)
                {
                    if (venueId > 0 && venueId != existingVenue.Id) // A new venue contact is assigned
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
            logger.InfoFormat("Event Segment deleted, Name: {0}, Id: {1}", "TODO", eventSegment.Id);
            return RedirectToAction("Index");
        }

        public JsonResult getAvailableTrainers(int segmentId)
        {
            EventSegment segment = db.EventSegments.Find(segmentId);

            List<EventSegment> overlappingSegs = db.EventSegments.Include(seg => seg.TrainorAllocations.Select(allo => allo.Trainor)).
                    Where(s => s.StartTime > segment.StartTime && s.EndTime < segment.EndTime).ToList();

            List<Trainor> allocatedTrainers = new List<Trainor>();
            foreach (var eventSegment in overlappingSegs)
            {
                foreach (var allocation in eventSegment.TrainorAllocations)
                {
                    allocatedTrainers.Add(allocation.Trainor);
                }
            }

            List<Trainor> availableTrainors = db.Trainors.ToList().Except(allocatedTrainers).ToList();
            return Json(availableTrainors, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getClientContacts(int eventId) 
        {
            Event evnt = db.Events.Include(x => x.Client).Where(z => z.Id == eventId).First();
            if (evnt.Client != null)
            {
                return Json(db.ClientContacts.Where(x => x.Client.Id == evnt.Client.Id).Select(i => new { i.ContactName, i.Id}), JsonRequestBehavior.AllowGet);
            } 
            else
            {
                return Json("A client Is not assigned to the event");
            }
        }

        public JsonResult getVenues()
        {
            return Json(db.Venues.Select(i => new {i.Id, i.VenueName}), JsonRequestBehavior.AllowGet);
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
