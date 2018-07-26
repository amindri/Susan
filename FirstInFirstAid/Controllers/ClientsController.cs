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
    public class ClientsController : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private FirstInFirstAidDBContext db = new FirstInFirstAidDBContext();

        public ActionResult ClientExists(string Name)
        {
            if (db.Clients.Any(x => x.Name == Name))
            {
                return Content("false");
            }
            else
            {
                return Content("true");
            }            
        }

        // GET: Clients
        public ActionResult Index()
        {
            logger.Debug("Getting the Clients list");
            return View(db.Clients.ToList());
        }

        // GET: Clients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Address,ClientContacts")]Client client)
        {
            if (ModelState.IsValid)
            {
                db.Clients.Add(client);
                db.SaveChanges();
                logger.InfoFormat("Client Created, Name : {0}, Id: {1}", client.Name, client.Id);
                return RedirectToAction("Index");

            }

            return View(client);
        }

        // GET: Clients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                logger.Warn("Received null Client Id to modify");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Include(c => c.ClientContacts).Include(a => a.Address).Where(x => x.Id == id).First();
            if (client == null)
            {
                logger.WarnFormat("Client not found to modify, Id: {0}", id);
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Address,ClientContacts")]Client client)
        {
            if (ModelState.IsValid)
            {
                logger.DebugFormat("Modifying Client of the Name: {0} and Id:{}", client.Name, client.Id);
                Client dbClient = db.Clients.Include(c => c.ClientContacts).Where(i => i.Id == client.Id).First();

                //updating the simple Client fields
                dbClient.Name = client.Name;

                //updating or adding the address
                if (client.Address != null)
                {
                    var dbAddress = db.Addresses.Find(client.Address.Id);
                    if (dbAddress != null)
                    {
                        db.Entry(dbAddress).CurrentValues.SetValues(client.Address);
                        db.Entry(dbAddress).State = EntityState.Modified;
                    }
                    else
                    {
                        dbClient.Address = client.Address;                        
                    }
                }                 

                //Deleting the deleted Client Contacts
                if (dbClient.ClientContacts != null)
                {
                    List<ClientContact> contactsToBeDeleted = new List<ClientContact>();
                    if (client.ClientContacts != null)
                    {
                        contactsToBeDeleted = (from clientContact in dbClient.ClientContacts
                                               let item = client.ClientContacts.SingleOrDefault(i => i.Id == clientContact.Id)
                                               where item == null
                                               select clientContact).ToList();
                    }
                    else
                    {
                        contactsToBeDeleted = dbClient.ClientContacts.ToList();
                    }

                    if (contactsToBeDeleted.Any())
                    {
                        foreach (var clientContact in contactsToBeDeleted.ToList())
                        {
                            db.Entry(clientContact).State = EntityState.Deleted;
                        }
                    }
                }
                
                if (client.ClientContacts != null)
                {
                    foreach (var clientContact in client.ClientContacts)
                    {
                        //Updating the existing client contacts
                        if (clientContact.Id > 0)
                        {
                            var clientContactDB = db.ClientContacts.Single(e => e.Id == clientContact.Id);
                            db.Entry(clientContactDB).CurrentValues.SetValues(clientContact);
                            db.Entry(clientContactDB).State = EntityState.Modified;
                        }
                        //Adding new Client contacts
                        else
                        {
                            dbClient.ClientContacts.Add(clientContact);
                        }
                    }
                }
                db.SaveChanges();
                logger.InfoFormat("Client modified, Name: {0}, Id: {1}", client.Name, client.Id);
                return RedirectToAction("Index");
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        public ActionResult Delete(int? id)
        {

            if (id == null)
            {
                logger.Warn("Received null Client Id to delete");

                return Content("{\"Type\":\"Warn\", \"Message\":\"Received null Client Id to delete\"}");

            }
            else
            {

                Client client = db.Clients.Include(e => e.Events).Where(i => i.Id == id).First();
                if (client == null)
                {
                    logger.WarnFormat("Client not found to delete, Id: {0}", id);
                    return Content("{\"Type\":\"Warn\", \"Message\":\"Client not found to delete with the Id:" + id + "\"}");
                }
                else if (client.Events.Count() > 0)
                {
                    return Content("{\"Type\":\"Warn\", \"Message\":\"The Client is associated with Events. Please delete the related Events or assign another Client to those Events\"}");
                }
                else
                {
                    return Content("{\"Type\":\"Confirm\", \"Message\":\"Are you sure you want to delete the Client: " + client.Name + "\", \"Id\": \" " + client.Id + "\"}");
                }
            }
            
        }

        [HttpGet]
        public JsonResult getNumberOfClients()
        {
            return Json(db.Clients.Count(), JsonRequestBehavior.AllowGet);
        }

        // POST: Clients/Delete/5
        [HttpPost]
        public JsonResult DeleteConfirmed(int id)
        {
            Client client = db.Clients.Include(a => a.Address).Include(c => c.ClientContacts).Where(i => i.Id == id).First();
            if (client.Address != null)
            {
                db.Addresses.Remove(client.Address);
            }            
            db.ClientContacts.RemoveRange(client.ClientContacts);
            db.Clients.Remove(client);
            db.SaveChanges();
            logger.InfoFormat("Client deleted, Name: {0}, Id: {1}", client.Name, client.Id);
            return Json("Successfully deleted the client: " + client.Name);
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
