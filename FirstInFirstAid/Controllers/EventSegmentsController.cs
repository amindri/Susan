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

namespace FirstInFirstAid.Controllers
{
    public class EventSegmentsController : Controller
    {
        private FirstInFirstAidDBContext db = new FirstInFirstAidDBContext();

        // GET: EventSegments
        public ActionResult Index()
        {
            var eventSegments = db.EventSegments;
            return View(eventSegments.ToList());
        }

        // GET: EventSegments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventSegment eventSegment = db.EventSegments.Find(id);
            if (eventSegment == null)
            {
                return HttpNotFound();
            }
            return View(eventSegment);
        }

        // GET: EventSegments/Create
        public ActionResult Create()
        {
            ViewBag.Id = new SelectList(db.ClientContacts, "Id", "ContactName");
            return View();
        }

        // POST: EventSegments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StartTime,EndTime,Hours,RequiredNumberOfstaff")] EventSegment eventSegment)
        {
            if (ModelState.IsValid)
            {
                db.EventSegments.Add(eventSegment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id = new SelectList(db.ClientContacts, "Id", "ContactName", eventSegment.Id);
            return View(eventSegment);
        }

        // GET: EventSegments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventSegment eventSegment = db.EventSegments.Find(id);
            if (eventSegment == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = new SelectList(db.ClientContacts, "Id", "ContactName", eventSegment.Id);
            return View(eventSegment);
        }

        // POST: EventSegments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StartTime,EndTime,Hours,RequiredNumberOfstaff")] EventSegment eventSegment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eventSegment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id = new SelectList(db.ClientContacts, "Id", "ContactName", eventSegment.Id);
            return View(eventSegment);
        }

        // GET: EventSegments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventSegment eventSegment = db.EventSegments.Find(id);
            if (eventSegment == null)
            {
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
            return RedirectToAction("Index");
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
