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
    public class TrainorsController : Controller
    {
        private FirstInFirstAidDBContext db = new FirstInFirstAidDBContext();

        // GET: Trainors
        public ActionResult Index()
        {
            return View(db.Trainors.ToList());
        }

        // GET: Trainors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trainor trainor = db.Trainors.Find(id);
            if (trainor == null)
            {
                return HttpNotFound();
            }
            return View(trainor);
        }

        // GET: Trainors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Trainors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,Lastname,PhoneNumber,Email,DOB,TaxFileNo")] Trainor trainor)
        {
            if (ModelState.IsValid)
            {
                db.Trainors.Add(trainor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(trainor);
        }

        // GET: Trainors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trainor trainor = db.Trainors.Find(id);
            if (trainor == null)
            {
                return HttpNotFound();
            }
            return View(trainor);
        }

        // POST: Trainors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,Lastname,PhoneNumber,Email,DOB,TaxFileNo")] Trainor trainor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trainor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(trainor);
        }

        // GET: Trainors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trainor trainor = db.Trainors.Find(id);
            if (trainor == null)
            {
                return HttpNotFound();
            }
            return View(trainor);
        }

        // POST: Trainors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Trainor trainor = db.Trainors.Find(id);
            db.Trainors.Remove(trainor);
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
