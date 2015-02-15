using System.Web.Mvc;

namespace OnlineMessenger.MvcServer.Tools
{
    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.RequestContext.HttpContext.Request;
            var response = filterContext.RequestContext.HttpContext.Response;

            if (request.UrlReferrer != null)
                response.AddHeader("Access-Control-Allow-Origin",
                    request.UrlReferrer.AbsoluteUri.Trim("/".ToCharArray()));
            response.AppendHeader("Access-Control-Allow-Methods", "GET,PUT,POST,DELETE");
            response.AppendHeader("Access-Control-Allow-Credentials", "true");
            response.AppendHeader("Access-Control-Allow-Headers", "Content-Type");
            base.OnActionExecuting(filterContext);
        }
    }
}
