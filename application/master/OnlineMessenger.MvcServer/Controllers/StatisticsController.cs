using System.Web.Mvc;
using OnlineMessenger.Domain.Entities;
using OnlineMessenger.Domain.Services;

namespace OnlineMessenger.MvcServer.Controllers
{
    [Authorize(Roles = Role.AdministratorRoleName)]
    public class StatisticsController : ControllerBase
    {
        private readonly IChartQueryBuilder _chartQueryBuilder;

        public StatisticsController(IChartQueryBuilder chartQueryBuilder)
        {
            _chartQueryBuilder = chartQueryBuilder;
        }

        public ActionResult Index()
        {
            return View();
        }

        #region jsoresults

        public JsonResult UsersPerDay()
        {
            return Json(_chartQueryBuilder.UsersPerDay());
        }

        public JsonResult MessagesPerDay()
        {
            return Json(_chartQueryBuilder.MessagesPerDay());
        }

        public JsonResult UsersPerOperator()
        {
            return Json(_chartQueryBuilder.UsersPerOperator());
        }

        #endregion jsoresults
    }
}