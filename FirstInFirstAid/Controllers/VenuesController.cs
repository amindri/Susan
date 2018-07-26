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
    public class VenuesController : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private FirstInFirstAidDBContext db = new FirstInFirstAidDBContext();

        public JsonResult VenueExists(string VenueName, int? Id)
        {
            var validateName = db.Venues.FirstOrDefault(x => x.VenueName == VenueName && x.Id != Id);

            if (validateName != null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Venues
        public ActionResult Index()
        {
            logger.Debug("Getting the Venue list");
            return View(db.Venues.ToList());
        }

        // GET: Venues/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,VenueName,Address")] Venue venue)
        {
            if (ModelState.IsValid)
            {                
                db.Venues.Add(venue);
                db.SaveChanges();
                logger.InfoFormat("Venue Created, Name : {0}, Id: {1}", venue.VenueName, venue.Id);
                return RedirectToAction("Index");                
            }
            return View(venue);
        }

        // GET: Venues/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                logger.Warn("Received null Venue Id to modify");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Venue venue = db.Venues.Include(s => s.Address).Where(x => x.Id == id).First();
            if (venue == null)
            {
                logger.WarnFormat("Venue not found to modify, Id: {0}", id);
                return HttpNotFound();
            }
            return View(venue);
        }

        // POST: Venues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,VenueName,Address")] Venue venue)
        {
            if (ModelState.IsValid)
            {
                logger.DebugFormat("Modifying Venue of the Name: {0} and Id:{}", venue.VenueName, venue.Id);
                db.Entry(venue).State = EntityState.Modified;
                if (venue.Address != null)
                {
                    if (venue.Address.Id > 0)
                    {
                        db.Entry(venue.Address).State = EntityState.Modified;
                    }
                    else
                    {
                        db.Entry(venue.Address).State = EntityState.Added;
                    }

                } 
                
                db.SaveChanges();
                logger.InfoFormat("Venue modified, Name: {0}, Id: {1}", venue.VenueName, venue.Id);
                return RedirectToAction("Index");
            }
            return View(venue);
        }

        // GET: Venues/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                logger.Warn("Received null Venue Id to delete");

                return Content("{\"Type\":\"Warn\", \"Message\":\"Received null Venue Id to delete\"}");

            }
            else
            {

                Venue venue = db.Venues.Include(e => e.EventSegments).Where(i => i.Id == id).First();
                if (venue == null)
                {
                    logger.WarnFormat("Venue not found to delete, Id: {0}", id);
                    return Content("{\"Type\":\"Warn\", \"Message\":\"Venue not found to delete with the Id:" + id + "\"}");
                }
                else if (venue.EventSegments.Count() > 0)
                {
                    return Content("{\"Type\":\"Warn\", \"Message\":\"The Venue is associated with one or more Event Segments. Please delete the related Event Segments or assign another Venue to those Event Segments\"}");
                }
                else
                {
                    return Content("{\"Type\":\"Confirm\", \"Message\":\"Are you sure you want to delete the Venue: " + venue.VenueName + "\", \"Id\": \" " + venue.Id + "\"}");
                }
            }
        }

        // POST: Venues/Delete/5
        [HttpPost]
        public JsonResult DeleteConfirmed(int id)
        {
            Venue venue = db.Venues.Find(id);
            db.Venues.Remove(venue);
            db.SaveChanges();
            logger.InfoFormat("Venue deleted, Name: {0}, Id: {1}", venue.VenueName, venue.Id);
            return Json("Successfully deleted the venue: " + venue.VenueName);
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
