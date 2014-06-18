using System.Configuration;
using Ninject;
using Ninject.Modules;
using Notifications.API;
using Notifications.Handlers;
using OmniCommon.Interfaces;
using Retrofit.Net;

namespace Notifications
{
    public class NotificationsModule : NinjectModule
    {
        private readonly string _baseUrl;

        public NotificationsModule()
        {
            _baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        }

        public override void Load()
        {
            Kernel.Bind<IHandler, INotificationsHandler>().To<NotificationsHandler>().InSingletonScope();

            Kernel.Bind<INotificationsApi>().ToMethod(c => GetApiEndpoint<INotificationsApi, Models.Notification>());
        }

        private T GetApiEndpoint<T, TR>()
            where T : class
            where TR : class
        {
            var authenticator = Kernel.Get<Authenticator>();
            var restAdapter = new RestAdapter(_baseUrl, authenticator);

            return restAdapter.Create<T, TR>();
        }
    }
}