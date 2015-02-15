using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using OnlineMessenger.Domain.Entities;

namespace OnlineMessenger.MvcServer.Tools
{
    /// <summary>
    /// Custom jsonresult, the main aim of which to prevent cycle referencing
    /// </summary>
    public class JsonNetResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            var response = context.HttpContext.Response;
            response.ContentType = !String.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;
            if (Data == null)
                return;

            var jsonResolver = new IgnorableSerializerContractResolver();
            jsonResolver.Ignore(typeof(User), "PasswordHash");

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = jsonResolver
            };
            var serializedObject = JsonConvert.SerializeObject(Data, Formatting.Indented, settings);
            response.Write(serializedObject);
        }
    }
}