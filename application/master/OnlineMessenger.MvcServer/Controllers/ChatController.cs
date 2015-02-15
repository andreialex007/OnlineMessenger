using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using OnlineMessenger.Domain.Entities;
using OnlineMessenger.Domain.Services;
using OnlineMessenger.MvcServer.Tools;
using OnlineMessenger.MvcServer.ViewModels;
using SixPack.Drawing;

namespace OnlineMessenger.MvcServer.Controllers
{
    public class ChatController : ControllerBase
    {

        #region private fields

        private const string CaptchaKey = "captcha";
        private const string CaptchaImageMimeType = "image/png";
        private const int CaptchaWidth = 40;
        private const int CaptchaHeight = 40;
        private readonly ImageFormat CaptchaImageMimeFormat = ImageFormat.Png;
        private readonly IMessageServer _messageServer;

        #endregion private fields

        #region constructor

        public ChatController(IMessageServer messageServer)
        {
            _messageServer = messageServer;
        }


        #endregion constructor

        #region ActionResults

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [AllowCrossSiteJson]
        public ActionResult UserPage()
        {
            if (!_messageServer.IsAnyOperatorOnline())
                return MailView();

            if (!User.Identity.IsAuthenticated)
            {
                var user = _messageServer.CreateClient();
                SignIn(user);
                _messageServer.AddNewClient(user);
                return View();
            }
            if (User.IsInRole(Role.OperatorRoleName) || User.IsInRole(Role.AdministratorRoleName))
            {
                SignOut();
                return RedirectPermanent("~/");
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult MailView()
        {
            var captcha = new CaptchaImage(Utils.GetRandomString(), 100, 50);
            captcha.Generate();
            return View("MailView");
        }

        #endregion ActionResults

        #region JsonResults

        [AllowAnonymous]
        public JsonResult SendMail(string Email, string UserName, string Message, string Code)
        {
            if (Code.ToLower() != Session[CaptchaKey].ToString().ToLower())
                return Json(new { Success = false });
            try
            {
                _messageServer.SendMail(UserName, Email, Message);
                return Json(new { Success = true });
            }
            catch (Exception)
            {
                return Json(new { Success = true });
            }
        }

        public JsonResult AddOperatorToVisible(string clientName)
        {
            try
            {
                _messageServer.AddOperatorToVisible(clientName, User.Identity.Name);
                return Json(new { Success = true });
            }
            catch (Exception)
            {
                return Json(new { Success = false });
            }
        }

        public JsonResult RemoveOperatorFromVisible(string clientName)
        {
            try
            {
                _messageServer.RemoveOperatorFromVisible(clientName, User.Identity.Name);
                return Json(new { Success = true });
            }
            catch (Exception)
            {
                return Json(new { Success = false });
            }
        }

        [HttpPost]
        public JsonResult SearchUsersByName(string name)
        {
            var users = ExcludeCurrentUser(_messageServer.SearchUsersByName(name)).Select(x =>
                new { x.Id, x.Name, x.IsConnected });
            return Json(users);
        }

        [HttpPost]
        public JsonResult GetMessagesOfUsers(string userName)
        {
            var messages = _messageServer.GetMessagesOfUsers(User.Identity.Name, userName);
            return Json(messages.Select(Mapper.Map<MessageViewModel>));
        }

        [HttpPost]
        public JsonResult GetNewMessages(int messagesCount)
        {
            var messages = _messageServer.GetNewMessagesForUser(User.Identity.Name, messagesCount);
            return Json(messages);
        }

        [HttpPost]
        public JsonResult AllGroups()
        {
            var visibleusers = _messageServer.GetVisibleUsersForOperator(User.Identity.Name);
            return
                Json(new
                    {
                        Users = ExcludeCurrentUser(visibleusers),
                        Operators = ExcludeCurrentUser(_messageServer.GetAdministratorsAndOperators())
                    });
        }

        [HttpPost]
        [AllowAnonymous]
        [AllowCrossSiteJson]
        public JsonResult AddMessageFromClient(string text)
        {
            try
            {
                _messageServer.SendClientMessage(User.Identity.Name, text);
                return Json(new { Success = true });
            }
            catch (Exception)
            {
                return Json(new { Success = false });
            }
        }

        [AllowAnonymous]
        [AllowCrossSiteJson]
        public JsonResult GetClientMessages()
        {
            var result = _messageServer.GetClientMessages(User.Identity.Name);
            return Json(result);
        }

        public JsonResult Send(string userName, string text)
        {
            try
            {
                _messageServer.Send(User.Identity.Name, userName, text);
                return Json(new { Success = true });
            }
            catch (Exception)
            {
                return Json(new { Success = false });
            }
        }

        #endregion JsonResults

        #region FileResults

        [AllowAnonymous]
        public FileResult GetCaptcha()
        {
            var code = Utils.GetRandomString();
            Session[CaptchaKey] = code;
            var captcha = new CaptchaImage(code, CaptchaWidth, CaptchaHeight);
            return File(captcha.Generate().ToStream(CaptchaImageMimeFormat), CaptchaImageMimeType);
        }

        #endregion FileResults

        #region private methods

        private IEnumerable<User> ExcludeCurrentUser(IEnumerable<User> users)
        {
            return users.Where(x => x.Name != User.Identity.Name);
        }

        #endregion private methods
    }
}