using System.Web.Routing;
using AttributeRouting.Web.Mvc;
using OnlineMessenger.MvcServer.App_Start;

[assembly: WebActivator.PreApplicationStartMethod(typeof(AttributeRoutingConfig), "Start")]

namespace OnlineMessenger.MvcServer.App_Start 
{
    public static class AttributeRoutingConfig
	{
		public static void RegisterRoutes(RouteCollection routes) 
		{    
			routes.MapAttributeRoutes();
		}

        public static void Start() 
		{
            RegisterRoutes(RouteTable.Routes);
        }
    }
}
