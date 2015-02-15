using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;


[assembly: OwinStartup(typeof(OnlineMessenger.MvcServer.Hubs.Startup))]
namespace OnlineMessenger.MvcServer.Hubs
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/LogIn")
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);


            app.MapSignalR(new HubConfiguration { EnableJSONP = true });

        }
    }
}