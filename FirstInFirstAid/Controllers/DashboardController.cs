using System.Web.Mvc;

namespace FirstInFirstAid.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }

    }
       
}
