using FirstInFirstAid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FirstInFirstAid.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {

            }

            System.Console.WriteLine("Hello World");


            return View("Dashoard");
        }
    }
}