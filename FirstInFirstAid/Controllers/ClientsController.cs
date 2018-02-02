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
    public class ClientsController : Controller
    {
        private FirstInFirstAidDBContext db = new FirstInFirstAidDBContext();

        // GET: Clients
        public ActionResult Index()
        {
            return View(db.Clients.ToList());
        }

        // GET: Clients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
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
        public ActionResult Create(Client client)
        {
            if (ModelState.IsValid)
            {
                db.Clients.Add(client);
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = client.Id });
            }

            return View(client);
        }

        // GET: Clients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Include(c => c.ClientContacts).Where(x => x.Id == id).First();
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Client client)
        {
            if (ModelState.IsValid)
            {
                Client dbClient = db.Clients.Include(c => c.ClientContacts).Where(i => i.Id == client.Id).First();

                //updating the client Name
                dbClient.Name = client.Name;

                if (dbClient.ClientContacts != null)
                { 
                    //Deleting the deleted contacts
                    var contactsToBeDeleted = (from email in dbClient.ClientContacts
                                                    let item = client.ClientContacts.SingleOrDefault(i => i.Id == email.Id)
                                                    where item == null
                                                    select email).ToList();
                    if (contactsToBeDeleted.Any())
                    {
                        foreach (var clientContact in contactsToBeDeleted.ToList())
                        {
                            db.Entry(clientContact).State = EntityState.Deleted;
                        }
                    }

                    //Updating the existing client contacts
                    foreach (var clientContact in client.ClientContacts)
                    {
                        if (clientContact.Id  > 0)
                        {
                            var clientContactDB = db.ClientContacts.Single(e => e.Id == clientContact.Id);
                            db.Entry(clientContactDB).CurrentValues.SetValues(clientContact);
                            db.Entry(clientContactDB).State = EntityState.Modified;
                        } else
                        {
                            db.ClientContacts.Add(clientContact);
                        }
                    }
                }               
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Client client = db.Clients.Find(id);
            db.Clients.Remove(client);
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
