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
    [Authorize]
    public class TrainorsController : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private FirstInFirstAidDBContext db = new FirstInFirstAidDBContext();

        // GET: Trainors
        public ActionResult Index()
        {
            logger.Debug("Getting the Trainer list");
            return View(db.Trainors.ToList());
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
                logger.InfoFormat("Trainer Created, Name : {0}, Id: {1}", trainor.FirstName, trainor.Id);
                return RedirectToAction("Index");
            }

            return View(trainor);
        }

        // GET: Trainors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                logger.Warn("Received null Trainer Id to modify");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trainor trainor = db.Trainors.Include(x => x.Qualifications).Include(y => y.Address).Where(z => z.Id == id).First();
            if (trainor == null)
            {
                logger.WarnFormat("Trainer not found to modify, Id: {0}", id);
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
                logger.DebugFormat("Modifying trainer of the Name: {0} and Id:{}", trainer.FirstName, trainer.Id);
                Trainor dbTrainer = db.Trainors.Include(c => c.Qualifications).Include(a => a.Address).Where(i => i.Id == trainer.Id).First();

                //updating the simple trainer fields
                dbTrainer.FirstName = trainer.FirstName;
                dbTrainer.Lastname = trainer.Lastname;
                dbTrainer.PhoneNumber = trainer.PhoneNumber;
                dbTrainer.TaxFileNo = trainer.TaxFileNo;
                dbTrainer.Email = trainer.Email;
                dbTrainer.DOB = trainer.DOB;

                
                if (trainer.Address != null)                    
                {
                    var dbAddress = db.Addresses.Find(trainer.Address.Id);
                    if (dbAddress != null)
                    {
                        db.Entry(dbAddress).CurrentValues.SetValues(trainer.Address);
                        db.Entry(dbAddress).State = EntityState.Modified;
                    }
                    else
                    {
                        dbTrainer.Address = trainer.Address;                        
                    }
                } else if (dbTrainer.Address != null)
                {
                    db.Entry(trainer.Address).State = EntityState.Deleted;
                }

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
                
                if (trainer.Qualifications != null) {
                    foreach (var qualification in trainer.Qualifications)
                    {
                        //Updating the existing qualifications
                        if (qualification.Id > 0)
                        {
                            var qualificationDB = db.Qualifications.Single(e => e.Id == qualification.Id);
                            db.Entry(qualificationDB).CurrentValues.SetValues(qualification);
                            db.Entry(qualificationDB).State = EntityState.Modified;
                        }
                        //Adding new qualification
                        else
                        {
                            dbTrainer.Qualifications.Add(qualification);
                        }
                    }
                }
                db.SaveChanges();
                logger.InfoFormat("Trainer modified, Name: {0}, Id: {1}", dbTrainer.FirstName, trainer.Id);
                return RedirectToAction("Index");
            }
            return View(trainer);
        }

        // GET: Trainors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                logger.Warn("Received null Trainer Id to delete");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trainor trainor = db.Trainors.Find(id);
            if (trainor == null)
            {
                logger.WarnFormat("Trainer not found to delete, Id: {0}", id);
                return HttpNotFound();
            }
            return View(trainor);
        }

        // POST: Trainors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Trainor trainor = db.Trainors.Include(q => q.Qualifications).Include(a => a.Address).Where(t => t.Id == id).First();
            db.Qualifications.RemoveRange(trainor.Qualifications);
            db.Addresses.Remove(trainor.Address);
            db.Trainors.Remove(trainor);
            db.SaveChanges();
            logger.InfoFormat("Trainer deleted, Name: {0}, Id: {1}", trainor.FirstName, trainor.Id);
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
