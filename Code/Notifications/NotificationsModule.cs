﻿using System.Configuration;
using Ninject;
using Ninject.Modules;
using Notifications.API;
using Notifications.Handlers;
using Notifications.NotificationList;
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
            Kernel.Bind<IOmniMessageHandler>().To<NotificationsHandler>().InSingletonScope();
            Kernel.Bind<INotificationsAPI>().ToMethod(c => this.GetApiEndpoint<INotificationsAPI, Models.Notification>());

            Kernel.Bind<INotificationListViewModel>().To<NotificationListViewModel>();
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