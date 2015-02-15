using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using OnlineMessenger.Data.Ef.Models;
using OnlineMessenger.Domain.Infrastructure;
using OnlineMessenger.Domain.Services;
using OnlineMessenger.MvcServer.Hubs;

namespace OnlineMessenger.MvcServer.Tools
{
    public class Bootstrapper
    {
        private static IWindsorContainer perRequestContainer;
        private static IWindsorContainer perThreadContainer;

        public static void BootstrapContainer()
        {
            //per request registration
            perRequestContainer = new WindsorContainer().Install(FromAssembly.This());
            perRequestContainer.Register(new IRegistration[]
                                       {
                                           Component.For<IMessageServer>().ImplementedBy<MessageServer>().LifestylePerWebRequest(),
                                           Component.For<IUnitOfWork>().ImplementedBy<UnitOfWork>().LifestylePerWebRequest(),
                                           Component.For<IBroadcaster>().ImplementedBy<Broadcaster>().LifestylePerWebRequest(),
                                           Component.For<IAccountManger>().ImplementedBy<AccountManager>().LifestylePerWebRequest(),
                                           Component.For<IMailer>().ImplementedBy<Mailer.Mailer>().LifestylePerWebRequest(),
                                           Component.For<IChartQueryBuilder>().ImplementedBy<ChartQueryBuilder>().LifestylePerWebRequest(),
                                           Component.For<MessengerDbContext>().LifestylePerWebRequest()
                                       });
            var controllerFactory = new WindsorControllerFactory(perRequestContainer.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);


            //per thread registration
            perThreadContainer = new WindsorContainer().Install(FromAssembly.This());
            perThreadContainer.Register(new IRegistration[]
                                       {
                                           Component.For<IMessageServer>().ImplementedBy<MessageServer>().LifestylePerThread(),
                                           Component.For<IUnitOfWork>().ImplementedBy<UnitOfWork>().LifestylePerThread(),
                                           Component.For<IBroadcaster>().ImplementedBy<Broadcaster>().LifestylePerThread(),
                                           Component.For<IAccountManger>().ImplementedBy<AccountManager>().LifestylePerThread(),
                                           Component.For<IMailer>().ImplementedBy<Mailer.Mailer>().LifestylePerThread(),
                                           Component.For<IChartQueryBuilder>().ImplementedBy<ChartQueryBuilder>().LifestylePerThread(),
                                           Component.For<MessengerDbContext>().LifestylePerThread()
                                       });

        }

        public static void Dispose()
        {
            perRequestContainer.Dispose();
            perThreadContainer.Dispose();
        }

        public static T Resolve<T>()
        {
            return perThreadContainer.Resolve<T>();
        }
    }
}