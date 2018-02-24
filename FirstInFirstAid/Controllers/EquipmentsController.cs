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
    public class EquipmentsController : Controller
    {
        private FirstInFirstAidDBContext db = new FirstInFirstAidDBContext();

        // GET: Equipments
        public ActionResult Index()
        {
            return View(db.Equipment.ToList());
        }

        // GET: Equipments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = db.Equipment.Find(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }
            return View(equipment);
        }

        // GET: Equipments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Equipments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EquipmentName,EquipmentAllocations")] Equipment equipment)
        {
            if (ModelState.IsValid)
            {
                db.Equipment.Add(equipment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(equipment);
        }

        // GET: Equipments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = db.Equipment.Find(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }
            return View(equipment);
        }

        // POST: Equipments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EquipmentName,EquipmentAllocations")] Equipment equipment)
        {
            if (ModelState.IsValid)
            {
                Equipment dbEquipment = db.Equipment.Include(c => c.EquipmentAllocations).Where(i => i.Id == equipment.Id).First();

                //updating the Equipment fields
                dbEquipment.EquipmentName = equipment.EquipmentName;

                //Deleting the deleted allocations
                if (dbEquipment.EquipmentAllocations != null)
                {
                    List<EquipmentAllocation> allocationsToBeDeleted = new List<EquipmentAllocation>();
                    if (equipment.EquipmentAllocations != null)
                    {
                        allocationsToBeDeleted = (from equipmentAllocation in dbEquipment.EquipmentAllocations
                                               let item = equipment.EquipmentAllocations.SingleOrDefault(i => i.Id == equipmentAllocation.Id)
                                               where item == null
                                               select equipmentAllocation).ToList();
                    }
                    else
                    {
                        allocationsToBeDeleted = dbEquipment.EquipmentAllocations.ToList();
                    }

                    if (allocationsToBeDeleted.Any())
                    {
                        foreach (var clientContact in allocationsToBeDeleted.ToList())
                        {
                            db.Entry(clientContact).State = EntityState.Deleted;
                        }
                    }
                }
                
                if (equipment.EquipmentAllocations != null)
                {
                    foreach (var equipmentAllocation in equipment.EquipmentAllocations)
                    {
                        //Updating the existing allocations
                        if (equipmentAllocation.Id > 0)
                        {
                            var equipmentAllocationDB = db.EquipmentAllocations.Single(e => e.Id == equipmentAllocation.Id);
                            db.Entry(equipmentAllocationDB).CurrentValues.SetValues(equipmentAllocation);
                            db.Entry(equipmentAllocationDB).State = EntityState.Modified;
                        }
                        //Adding new allocations
                        else
                        {
                            dbEquipment.EquipmentAllocations.Add(equipmentAllocation);
                        }
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(equipment);
        }

        // GET: Equipments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = db.Equipment.Find(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }
            return View(equipment);
        }

        // POST: Equipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Equipment equipment = db.Equipment.Find(id);
            db.Equipment.Remove(equipment);
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
