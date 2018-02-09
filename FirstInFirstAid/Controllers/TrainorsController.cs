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
        public ActionResult Create([Bind(Include = "Id,FirstName,Lastname,PhoneNumber,Email,DOB,TaxFileNo,Address,Qualifications")] Trainor trainor)
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
            Trainor trainor = db.Trainors.Include(x => x.Qualifications).Include(y => y.Address).Where(z => z.Id == id).First();
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
        public ActionResult Edit([Bind(Include = "Id,FirstName,Lastname,PhoneNumber,Email,DOB,TaxFileNo,Address,Qualifications")] Trainor trainer)
        {
            if (ModelState.IsValid)
            {
                Trainor dbTrainer = db.Trainors.Include(c => c.Qualifications).Where(i => i.Id == trainer.Id).First();

                //updating the client Name
                dbTrainer.FirstName = trainer.FirstName;
                dbTrainer.Lastname = trainer.Lastname;
                dbTrainer.PhoneNumber = trainer.PhoneNumber;
                dbTrainer.TaxFileNo = trainer.TaxFileNo;
                dbTrainer.Email = trainer.Email;
                dbTrainer.DOB = trainer.DOB;
                db.Entry(trainer.Address).State = EntityState.Modified;
                //Deleting the deleted qualifications
                if (dbTrainer.Qualifications != null)
                {
                    List<Qualification> qualificationsToBeDeleted = new List<Qualification>();
                    if (trainer.Qualifications != null)
                    {
                        qualificationsToBeDeleted = (from qualification in dbTrainer.Qualifications
                                           let item = trainer.Qualifications.SingleOrDefault(i => i.Id == qualification.Id)
                                           where item == null
                                           select qualification).ToList();
                    } else
                    {
                        qualificationsToBeDeleted = dbTrainer.Qualifications.ToList();
                    }

                    if (qualificationsToBeDeleted.Any())
                    {
                        foreach (var clientContact in qualificationsToBeDeleted.ToList())
                        {
                            db.Entry(clientContact).State = EntityState.Deleted;
                        }
                    }             
                }
                //Updating the existing qualifications
                if (trainer.Qualifications != null) {
                    foreach (var qualification in trainer.Qualifications)
                    {
                        if (qualification.Id > 0)
                        {
                            var qualificationDB = db.Qualifications.Single(e => e.Id == qualification.Id);
                            db.Entry(qualificationDB).CurrentValues.SetValues(qualification);
                            db.Entry(qualificationDB).State = EntityState.Modified;
                        }
                        else
                        {
                            dbTrainer.Qualifications.Add(qualification);
                        }
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(trainer);
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
