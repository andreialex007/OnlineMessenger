using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using OnlineMessenger.Domain.Services;
using OnlineMessenger.MvcServer.ViewModels;

namespace OnlineMessenger.MvcServer.Controllers
{
    public class MessagesController : ControllerBase
    {
        private readonly IMessageServer _messageServer;

        public MessagesController(IMessageServer messageServer)
        {
            _messageServer = messageServer;
        }

        public ActionResult Index()
        {
            return View();
        }

        #region JsonResults

        [HttpPost]
        public JsonResult SearchMessages(int[] Id, string[] From, DateTime? Date, string Text, string orderBy, int? take, int? skip, bool isAsc = false)
        {
            var keyValuePair = _messageServer.GetMessages(Id, From, Date, Text, orderBy, isAsc, take, skip);

            return Json(new
                        {
                            items = keyValuePair.Value.Select(Mapper.Map<MessageDisplayViewModel>),
                            total = keyValuePair.Key
                        });
        }

        [HttpPost]
        public JsonResult DeleteMessages(int[] ids)
        {
            _messageServer.DeleteMessages(ids);
            return Json(new { Success = true });
        }

        #endregion JsonResults

        #region protected methods

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _messageServer.Dispose();
        }

        #endregion protected methods

    }
}