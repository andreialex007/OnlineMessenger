using System.Web;
using System.Web.Mvc;

namespace OnlineMessenger.MvcServer.Tools
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Converts url to absolute path including web site domain
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="contentPath">path to convert</param>
        /// <returns></returns>
        public static string Absolute(this HtmlHelper htmlHelper, string contentPath)
        {
            var httpContext = htmlHelper.ViewContext.HttpContext;
            var path = VirtualPathUtility.ToAbsolute(contentPath);
            var siteUrl = httpContext.Request.Url.AbsoluteUri.TrimEnd(httpContext.Request.Url.AbsolutePath.ToCharArray());
            var fullContentPath = string.Format("{0}{1}", siteUrl, path);
            return fullContentPath;
        }
    }
}