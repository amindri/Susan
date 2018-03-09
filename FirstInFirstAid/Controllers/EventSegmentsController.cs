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

namespace FirstInFirstAid.Controllers
{
    public class EventSegmentsController : Controller
    {
        private FirstInFirstAidDBContext db = new FirstInFirstAidDBContext();
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            return Json("Invalid Model State");         
        }

        // GET: EventSegments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                logger.Warn("Received null Event Segment Id to modify");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventSegment eventSegment = db.EventSegments.Include(c => c.Event).Where(x => x.Id == id).First();
            if (eventSegment == null)
            {
                logger.Warn("Received null Event Segement Id to modify");
                return HttpNotFound();
            }
            return View(eventSegment);
        }

        // POST: EventSegments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,StartTime,EndTime,Hours,RequiredNumberOfstaff")] EventSegment eventSegment)
        {
            if (ModelState.IsValid)
            {
                logger.DebugFormat("Modifying Event Segment of the Name: {0} and Id:{}", eventSegment.Name, eventSegment.Id);
                db.Entry(eventSegment).State = EntityState.Modified;
                db.SaveChanges();
                logger.InfoFormat("Event Segment modified, Name: {0}, Id: {1}", eventSegment.Name, eventSegment.Id);
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
                return Json(db.ClientContacts.Where(x => x.Client.Id == evnt.Client.Id), JsonRequestBehavior.AllowGet);
            } 
            else
            {
                return Json("A client Is not assigned to the event");
            }
        }

        public JsonResult getVenues()
        {
            return Json(db.Venues, JsonRequestBehavior.AllowGet);
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
