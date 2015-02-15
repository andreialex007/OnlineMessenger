using System.Drawing;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using OnlineMessenger.Domain.Entities;
using OnlineMessenger.Domain.Exceptions;
using OnlineMessenger.MvcServer.Hubs;
using OnlineMessenger.MvcServer.ThreadingTools;
using OnlineMessenger.MvcServer.Tools;

namespace OnlineMessenger.MvcServer.Controllers
{
    [Authorize(Roles = Role.AdministratorRoleName + "," + Role.OperatorRoleName)]
    public class ControllerBase : Controller
    {
        private const string StringValueType = "http://www.w3.org/2001/XMLSchema#string";

        #region authentification

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public void SignIn(User user, bool isPersistent = false)
        {
            AuthenticationManager.SignIn();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Name, StringValueType));
            foreach (var role in user.Roles)
                identity.AddClaim(new Claim(ClaimTypes.Role, role.Name, StringValueType));
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, identity);
        }

        public void SignOut()
        {
            ConnectionTracker.ProcessConnection(User.Identity.Name);
            AuthenticationManager.SignOut();
        }

        #endregion authentification

        #region protected methods

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {

            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        protected void ThrowIfNotImage(HttpPostedFileBase fileBase)
        {
            var image = Image.FromStream(fileBase.InputStream, true, true);
            if (image == null)
                throw new ValidationException("Image", "Uploaded file is not image");
            image.Dispose();
        }

        #endregion protected methods
    }
}