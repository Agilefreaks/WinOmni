using System.Configuration;
using Ninject;
using Ninject.Modules;
using Ninject.Extensions.Conventions;
using Ninject.Extensions.Conventions.Syntax;
using Notifications.API;
using Notifications.Handlers;
using Notifications.Models;
using Notifications.NotificationList;
using OmniCommon.Interfaces;
using Retrofit.Net;

namespace Notifications
{
    public class NotificationsModule : NinjectModule
    {
        private string _baseUrl;

        public NotificationsModule()
        {
            _baseUrl = ConfigurationManager.AppSettings["baseUrl"];
        }

        public override void Load()
        {
            Kernel.Bind<IOmniMessageHandler>().To<NotificationsHandler>().InSingletonScope();
            Kernel.Bind<INotificationsAPI>().ToMethod(c => GetAPIEndpoint<INotificationsAPI, Models.Notification>());

            Kernel.Bind<INotificationListViewModel>().To<NotificationListViewModel>();
        }

        private T GetAPIEndpoint<T, R>()
            where T : class
            where R : class
        {
            var authenticator = Kernel.Get<Authenticator>();
            var restAdapter = new RestAdapter(_baseUrl, authenticator);

            return restAdapter.Create<T, R>();
        }
    }
}