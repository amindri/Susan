using System.Data.Entity;
using System.Web.Mvc;
using FirstInFirstAid.DAL;
using FirstInFirstAid.Models;
using log4net;
using System.Reflection;
using System.Linq;

namespace FirstInFirstAid.Controllers
{
    public class TrainorAllocationForEventSegsController : Controller
    {
        private FirstInFirstAidDBContext db = new FirstInFirstAidDBContext();
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // POST: TrainorAllocationForEventSegs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(TrainorAllocationForEventSeg trainorAllocation, int eventSegmentId, int trainerId)
        {
            if (ModelState.ContainsKey("trainorAllocation.EventSegment"))
            {
                ModelState["trainorAllocation.EventSegment"].Errors.Clear();
            }
            if (ModelState.IsValid)
            {
                EventSegment evntSegment = db.EventSegments.Include(c => c.TrainorAllocations).Where(i => i.Id == eventSegmentId).First();
                Trainor trainer = db.Trainors.Find(trainerId);
                trainorAllocation.Trainor = trainer;
                evntSegment.TrainorAllocations.Add(trainorAllocation);
                db.SaveChanges();
                logger.InfoFormat("Trainer Allocation created for the EventSegment : {0}, with the Trainer : {1}", evntSegment.Name, trainorAllocation.Trainor.FirstName  + " " + trainorAllocation.Trainor.Lastname);
                return Json(new
                {
                    Id = trainorAllocation.Id,
                    DutyType = (int)trainorAllocation.DutyType,
                    PresenceConfirmation = trainorAllocation.PresenceConfirmation,
                    Hours = trainorAllocation.Hours,
                    PaymentNote = trainorAllocation.PaymentNote,
                    Paid = trainorAllocation.Paid,
                    Trainor = trainorAllocation.Trainor.Id
                });
            }
            return Json("Invalid Model State");
        }

        // POST: TrainorAllocationForEventSegs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "Id,DutyType,PresenceConfirmation,PaymentNote,Hours,Paid")] TrainorAllocationForEventSeg trainorAllocation, int trainerId)
        {
            if (ModelState.ContainsKey("trainorAllocation.EventSegment"))
            {
                ModelState["trainorAllocation.EventSegment"].Errors.Clear();
            }
            if (ModelState.IsValid)
            {
                logger.DebugFormat("Modifying TrainerAllocationforEventSegment with the Trainer First Name: {0} {1},  and Id:{2}", 
                    trainorAllocation.Trainor?.FirstName, trainorAllocation.Trainor?.Lastname, trainorAllocation.Id);

                TrainorAllocationForEventSeg dbTrainerAllocation = db.TrainorEventSegAllocations.Include(s => s.EventSegment).Include(t => t.Trainor).Where(i => i.Id == trainorAllocation.Id).First();

                dbTrainerAllocation.Paid = trainorAllocation.Paid;
                dbTrainerAllocation.PaymentNote = trainorAllocation.PaymentNote;
                dbTrainerAllocation.PresenceConfirmation = trainorAllocation.PresenceConfirmation;
                dbTrainerAllocation.DutyType = trainorAllocation.DutyType;
                dbTrainerAllocation.Hours = trainorAllocation.Hours;
                dbTrainerAllocation.Trainor = db.Trainors.Find(trainerId);
                
                db.SaveChanges();
                logger.InfoFormat("Successfully updated the trainer allocation of the ID: {0}, with the Trainer : {1} {2}", dbTrainerAllocation.Id, dbTrainerAllocation.Trainor.FirstName, dbTrainerAllocation.Trainor.Lastname);
                return Json(new
                {
                    Id = dbTrainerAllocation.Id,
                    DutyType = (int)dbTrainerAllocation.DutyType,
                    PresenceConfirmation = dbTrainerAllocation.PresenceConfirmation,
                    Hours = dbTrainerAllocation.Hours,
                    PaymentNote = dbTrainerAllocation.PaymentNote,
                    Paid = dbTrainerAllocation.Paid,
                    Trainor = dbTrainerAllocation.Trainor.Id
                });
            }
            return Json("Invalid Model State");
        }

        public ActionResult Delete(int? id)
        {

            if (id == null)
            {
                logger.Warn("Received null Trainer Allocation Id to delete");

                return Content("{\"Type\":\"Warn\", \"Message\":\"Received null Trainer Allocation Id to delete\"}");

            }
            else
            {

                TrainorAllocationForEventSeg trainerAllocation = db.TrainorEventSegAllocations.Include(t => t.Trainor).Where(i => i.Id == id).First();
                if (trainerAllocation == null)
                {
                    logger.WarnFormat("Trainer Allocation not found to delete, Id: {0}", id);
                    return Content("{\"Type\":\"Warn\", \"Message\":\"Trainer Allocation not found to delete with the Id:" + id + "\"}");
                }                
                else
                {
                    return Content("{\"Type\":\"Confirm\", \"Message\":\"Are you sure you want to delete the Trainer Allocation, with the trainer  " + trainerAllocation.Trainor.FirstName + " " + trainerAllocation.Trainor.Lastname + "\", \"Id\": \" " + trainerAllocation.Id + "\"}");
                }
            }

        }

        // POST: TrainorAllocationForEventSegs/Delete/5
        public ActionResult DeleteConfirmed(int id)
        {
            TrainorAllocationForEventSeg trainorAllocationForEventSeg = db.TrainorEventSegAllocations.Find(id);
            trainorAllocationForEventSeg.Trainor = null;
            db.TrainorEventSegAllocations.Remove(trainorAllocationForEventSeg);
            db.SaveChanges();
            return Json("Successfully deleted the trainer allocation of the Id: " + id);
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
