using System.Web.Mvc;

namespace OnlineMessenger.MvcServer.Controllers
{
    public class CodeController : ControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}