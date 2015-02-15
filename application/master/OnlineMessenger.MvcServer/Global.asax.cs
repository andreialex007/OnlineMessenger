using System.Web.Mvc;
using System.Web.Routing;
using OnlineMessenger.Domain.Services;
using OnlineMessenger.MvcServer.App_Start;
using OnlineMessenger.MvcServer.Tools;
using OnlineMessenger.MvcServer.ViewModels;

namespace OnlineMessenger.MvcServer
{

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            UserEditViewModel.InitMap();
            RoleViewModel.InitMap();
            UserTableItemViewModel.InitMap();
            RoleTableItemViewModel.InitMap();
            RoleEditViewModel.InitMap();
            MessageViewModel.InitMap();
            UserSettingsEditModel.InitMap();
            MessageDisplayViewModel.InitMap();


            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            Bootstrapper.BootstrapContainer();

            Bootstrapper.Resolve<IMessageServer>().SetDisconnectToAll();
        }

        protected void Application_End()
        {
            Bootstrapper.Dispose();
        }

        protected void Application_BeginRequest()
        {
            if (System.Web.HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath == "~/signalr/abort")
            {
                
            }
        }
       
    }
}
