using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FirstInFirstAid.DAL;
using FirstInFirstAid.Models;
using System.Diagnostics;

namespace FirstInFirstAid.Controllers
{
    public class EventsController : Controller
    {
        private FirstInFirstAidDBContext db = new FirstInFirstAidDBContext();

        // GET: Events
        public ActionResult Index()
        {
            var events = db.Events.Include(c => c.Client);
            return View(events.ToList());
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event evenat = db.Events.Include(s => s.EventSegments).Where(x => x.Id == id).First();
            if (evenat == null)
            {
                return HttpNotFound();
            }
          
            return View(evenat);
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
        public ActionResult Create(Event ev3nt)
        {
            if (ModelState.IsValid)
            {                
                Event newEvent = db.Events.Add(ev3nt);
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = ev3nt.Id });
            }

            ViewBag.Id = new SelectList(db.Clients, "Id", "Name", ev3nt.Id);
            return View(ev3nt); 
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Include(s => s.EventSegments).Where(x => x.Id == id).First();
            if (@event == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = new SelectList(db.Clients, "Id", "Name", @event.Id);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EventName,InvoiceNumber,HourlyRate,TotalFee,EventState,EventSegments,Client")] Event @event)
        {
            if (ModelState.IsValid)
            {
                Event dbEvent = db.Events.Include(c => c.EventSegments).Where(i => i.Id == @event.Id).First();

                //updating the Event fields
                dbEvent.EventName = @event.EventName;
                dbEvent.EventState = @event.EventState;
                dbEvent.BusinessId = @event.BusinessId;
                dbEvent.HourlyRate = @event.HourlyRate;
                dbEvent.InvoiceNumber = @event.InvoiceNumber;
                dbEvent.TotalFee = @event.TotalFee;
                if (@event.Client != null) {
                    if (@event.Client.Id > 0)
                    {
                        db.Entry(@event.Client).State = EntityState.Modified;
                    }
                    else
                    {
                        db.Entry(@event.Client).State = EntityState.Added;
                    }

                }

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
                //Updating the existing eventSegments
                if (@event.EventSegments != null)
                {
                    foreach (var eventSegment in @event.EventSegments)
                    {
                        if (eventSegment.Id > 0)
                        {
                            var eventsegmentDB = db.EventSegments.Single(e => e.Id == eventSegment.Id);
                            db.Entry(eventsegmentDB).CurrentValues.SetValues(eventSegment);
                            db.Entry(eventsegmentDB).State = EntityState.Modified;
                        }
                        else
                        {
                            dbEvent.EventSegments.Add(eventSegment);
                        }
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(@event);
        }
        
        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event eventVar = db.Events.Find(id);
            if (eventVar == null)
            {
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
