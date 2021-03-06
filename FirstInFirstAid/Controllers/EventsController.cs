﻿using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Reflection;
using FirstInFirstAid.DAL;
using FirstInFirstAid.Models;
using log4net;
using System;
using System.Collections.Generic;

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
            //Select List for duty type
            var dutyType = from Coverage d in Enum.GetValues(typeof(Coverage))
                           select new { ID = (int)d, Name = d.ToString() };
            ViewBag.CoverageSelectList = dutyType.ToList();
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EventName,InvoiceNumber,HourlyRate,BusinessId,EventState")]Event ev3nt, int clientId)
        {                        

            if (ModelState.IsValid)
            {
                Client client = db.Clients.Include(e => e.Events).Where(i => i.Id == clientId).First();
                client.Events.Add(ev3nt);        
                db.SaveChanges();
                logger.InfoFormat("Event Created, Name : {0}, Id: {1}", ev3nt.EventName, ev3nt.Id);
                var id = ev3nt.Id;
                return RedirectToAction("Edit", new
                {
                    id = ev3nt.Id,
                });
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
            //Select List for duty type
            var dutyType = from Coverage d in Enum.GetValues(typeof(Coverage))
                           select new { ID = (int)d, Name = d.ToString() };
            ViewBag.CoverageSelectList = dutyType.ToList();
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EventName,InvoiceNumber,HourlyRate,BusinessId,EventState")] Event @event, int clientId)
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

                db.SaveChanges();
                logger.InfoFormat("Event successfully modified, Name: {0}, Id: {1}", dbEvent.EventName, @event.Id);
                return RedirectToAction("Index");
            }
            var clientList = from Client v in db.Clients select new { ID = v.Id, Name = v.Name };
            ViewBag.Clients = new SelectList(clientList, "ID", "Name", @event.Client.Id);
            return View(@event);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                logger.Warn("Received null Event Id to delete");

                return Content("{\"Type\":\"Warn\", \"Message\":\"Received null Event Id to delete\"}");

            }
            else
            {

                Event @event = db.Events.Include(e => e.EventSegments).Where(i => i.Id == id).First();
                if (@event == null)
                {
                    logger.WarnFormat("Event not found to delete, Id: {0}", id);
                    return Content("{\"Type\":\"Warn\", \"Message\":\"Event not found to delete with the Id:" + id + "\"}");
                }
                else if (@event.EventSegments.Count() > 0)
                {
                    return Content("{\"Type\":\"WarnConfirm\", \"Message\":\"Are you sure? This Event is associated with " + @event.EventSegments.Count + " Event Segment/s. Continuing will delete all of it.\", \"Id\": \" " + @event.Id + "\"}");
                }
                else
                {
                    return Content("{\"Type\":\"Confirm\", \"Message\":\"Are you sure you want to delete the Event: " + @event.EventName + "\", \"Id\": \" " + @event.Id + "\"}");
                }
            }
        }

        // POST: Events/Delete/5
        [HttpPost]
        public JsonResult DeleteConfirmed(int id)
        {
            Event eventVar = db.Events.Find(id);            
            db.Events.Remove(eventVar);
            db.SaveChanges();
            logger.InfoFormat("Event deleted, Name: {0}, Id: {1}", eventVar.EventName, eventVar.Id);
            return Json("Successfully deleted the Event: " + eventVar.EventName);
        }

        public JsonResult GetClients() // its a GET, not a POST
        {
            return Json(db.Clients, JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult getEventState()
        {
            List<object> list = new List<object>();
            foreach (Event ev3nt in db.Events)
            {                
                list.Add(new { ID = ev3nt.Id, State = ev3nt.EventState});                
            }

            return Json(list, JsonRequestBehavior.AllowGet);
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
