using System.Web.Mvc;

namespace OnlineMessenger.MvcServer.Controllers
{
    public class HomeController : ControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}