using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using OnlineMessenger.Domain.Entities;
using OnlineMessenger.Domain.Exceptions;
using OnlineMessenger.Domain.Services;
using OnlineMessenger.MvcServer.Hubs;
using OnlineMessenger.MvcServer.ThreadingTools;
using OnlineMessenger.MvcServer.Tools;
using OnlineMessenger.MvcServer.ViewModels;

namespace OnlineMessenger.MvcServer.Controllers
{
    public class AccountController : ControllerBase
    {
        private const string DefaultAvatarImagePath = "~/Content/images/user-default-avatar.png";
        private const string DefaultAvatarImageMime = "image/png";

        private readonly IAccountManger _accountManager;

        public AccountController(IAccountManger accountManager)
        {
            _accountManager = accountManager;
        }

        #region ActionResults

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult LogIn()
        {
            return View();
        }

        public ActionResult LogOut()
        {
            var userName = User.Identity.Name;
            SignOut();
            ConnectionTracker.ProcessDisconnection(userName);
            return Redirect(Url.Content("~/"));
        }

        #endregion ActionResults

        #region JsonResults

        [HttpPost]
        public JsonResult UploadAvatar(HttpPostedFileBase file)
        {
            try
            {
                ThrowIfNotImage(file);
                _accountManager.UploadUserAvatar(User.Identity.Name, file.ToByteArray());
                return Json(new { Success = true });
            }
            catch (ValidationException exception)
            {
                return Json(new { Success = true, Errors = exception.ValidationErrors });
            }
        }

        [HttpPost]
        public JsonResult GetUserSettings()
        {
            var currentUser = _accountManager.GetUserByName(User.Identity.Name);
            return Json(Mapper.Map<UserSettingsEditModel>(currentUser));
        }

        [HttpPost]
        public JsonResult GetMessengerSettings()
        {
            var currentUser = _accountManager.GetUserByName(User.Identity.Name);
            return Json(new
                        {
                            VisualNotificationsEnabled = currentUser.VisualNotificationsEnabled ?? false,
                            AudioNotificationsEnabled = currentUser.AudioNotificationsEnabled ?? false
                        });
        }

        [HttpPost]
        public JsonResult EditUserSettings(UserSettingsEditModel model)
        {
            try
            {
                model.Name = User.Identity.Name;
                _accountManager.UpdateUserSettings(Mapper.Map<User>(model));
                if (model.IsChangePassword)
                {
                    _accountManager.ChangePassword(model.Name, model.CurrentPassword, model.NewPassword,
                        model.PasswordConfirm);
                }
                return Json(new { Success = true });
            }
            catch (ValidationException exception)
            {
                return Json(new { Success = false, Errors = exception.ValidationErrors });
            }
        }

        [AllowAnonymous]
        public JsonResult TryLogin(string userName, string password, bool isPersist)
        {
            var user = _accountManager.CheckPassword(userName, password);
            if (user != null)
            {
                SignIn(user, isPersist);
                ConnectionTracker.ProcessConnection(user.Name);
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }

        #endregion JsonResults

        #region FileResults

        [AllowAnonymous]
        public FileResult GetAvatar(string userName)
        {
            var bytes = _accountManager.GetUserAvatar(userName ?? User.Identity.Name);
            if (bytes == null)
                return File(Url.Content(DefaultAvatarImagePath), DefaultAvatarImageMime);
            var stream = new MemoryStream(bytes);
            var image = Image.FromStream(stream);
            return File(bytes, image.RawFormat.GetMimeType());
        }

        #endregion FileResult
    }
}