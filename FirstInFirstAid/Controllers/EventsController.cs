using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Reflection;
using FirstInFirstAid.DAL;
using FirstInFirstAid.Models;
using log4net;

namespace FirstInFirstAid.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private FirstInFirstAidDBContext db = new FirstInFirstAidDBContext();

        // GET: Events
        public ActionResult Index()
        {
            logger.Debug("Getting the Event list");
            var events = db.Events.Include(c => c.Client);
            return View(events.ToList());
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            ViewBag.Id = new SelectList(db.Clients, "Id", "Name");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EventName,InvoiceNumber,HourlyRate,TotalFee,BusinessId,EventState,EventSegments")]Event ev3nt, int clientId)
        {
            if (ModelState.IsValid)
            {
                Client client = db.Clients.Include(e => e.Events).Where(i => i.Id == clientId).First();
                client.Events.Add(ev3nt);
                if (ev3nt.EventSegments != null)
                {
                    foreach (EventSegment segment in ev3nt.EventSegments)
                    {
                        segment.Hours = (segment.EndTime - segment.StartTime).Hours;
                    }
                }
                db.SaveChanges();
                logger.InfoFormat("Event Created, Name : {0}, Id: {1}", ev3nt.EventName, ev3nt.Id);
                return RedirectToAction("Index");

            }
            ViewBag.Id = new SelectList(db.Clients, "Id", "Name");
            return View(ev3nt);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                logger.Warn("Received null Event Id to modify");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Include(s => s.EventSegments).Include(c => c.Client).Where(x => x.Id == id).First();
            if (@event == null)
            {
                logger.Warn("Received null Event Id to modify");
                return HttpNotFound();
            }
            var clientList = from Client v in db.Clients select new { ID = v.Id, Name = v.Name };
            ViewBag.Clients = new SelectList(clientList, "ID", "Name", @event.Client.Id);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EventName,InvoiceNumber,HourlyRate,TotalFee,BusinessId,EventState,EventSegments")] Event @event, int clientId)
        {
            if (ModelState.IsValid)
            {
                logger.DebugFormat("Modifying Event of the Name: {0} and Id:{}", @event.EventName, @event.Id);
                Event dbEvent = db.Events.Include(c => c.EventSegments).Include(x => x.Client).Where(i => i.Id == @event.Id).First();

                //Updating the client
                Client existingClient = dbEvent.Client;
                if (existingClient != null) { 
                    if (clientId > 0 && clientId != existingClient.Id) {
                        Client newClient = db.Clients.Include(e => e.Events).Where(i => i.Id == clientId).First();
                        existingClient.Events.Remove(dbEvent);
                        newClient.Events.Add(dbEvent);
                    } 
                } else if (clientId > 0) 
                {
                    Client newClient = db.Clients.Include(e => e.Events).Where(i => i.Id == clientId).First();
                    newClient.Events.Add(dbEvent);
                }

                //updating the Event fields
                dbEvent.EventName = @event.EventName;
                dbEvent.EventState = @event.EventState;
                dbEvent.BusinessId = @event.BusinessId;
                dbEvent.HourlyRate = @event.HourlyRate;
                dbEvent.InvoiceNumber = @event.InvoiceNumber;
                dbEvent.TotalFee = @event.TotalFee;

                //Deleting the deleted Segments
                if (dbEvent.EventSegments != null)
                {
                    List<EventSegment> segmentsToBeDeleted = new List<EventSegment>();
                    if (@event.EventSegments != null)
                    {
                        segmentsToBeDeleted = (from eventSegment in dbEvent.EventSegments
                                               let item = @event.EventSegments.SingleOrDefault(i => i.Id == eventSegment.Id)
                                               where item == null
                                               select eventSegment).ToList();
                    }
                    else
                    {
                        segmentsToBeDeleted = dbEvent.EventSegments.ToList();
                    }

                    if (segmentsToBeDeleted.Any())
                    {
                        foreach (var eventSegment in segmentsToBeDeleted.ToList())
                        {
                            db.Entry(eventSegment).State = EntityState.Deleted;
                        }
                    }
                }
                
                if (@event.EventSegments != null)
                {
                    foreach (var eventSegment in @event.EventSegments)
                    {
                        //Updating the existing eventSegments
                        if (eventSegment.Id > 0)
                        {
                            var eventsegmentDB = db.EventSegments.Single(e => e.Id == eventSegment.Id);
                            db.Entry(eventsegmentDB).CurrentValues.SetValues(eventSegment);
                            db.Entry(eventsegmentDB).State = EntityState.Modified;
                        }
                        //Adding new event segments
                        else
                        {
                            dbEvent.EventSegments.Add(eventSegment);
                        }
                    }
                }
                db.SaveChanges();
                logger.InfoFormat("Event successfully modified, Name: {0}, Id: {1}", dbEvent.EventName, @event.Id);
                return RedirectToAction("Index");
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                logger.Warn("Received null Event Id to delete");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event eventVar = db.Events.Find(id);
            if (eventVar == null)
            {
                logger.WarnFormat("Event not found to delete, Id: {0}", id);
                return HttpNotFound();
            }
            return View(eventVar);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event eventVar = db.Events.Find(id);
            db.Events.Remove(eventVar);
            db.SaveChanges();
            logger.InfoFormat("Event deleted, Name: {0}, Id: {1}", eventVar.EventName, eventVar.Id);
            return RedirectToAction("Index");
        }

        public JsonResult GetClients() // its a GET, not a POST
        {
            return Json(db.Clients, JsonRequestBehavior.AllowGet);
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
