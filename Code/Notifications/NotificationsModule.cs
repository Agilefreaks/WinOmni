using Ninject.Modules;
using Notifications.Handlers;
using OmniCommon.Interfaces;

namespace Notifications
{
    using Notifications.Api.Resources.v1;

    public class NotificationsModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<INotifications>().To<Notifications>().InSingletonScope();

            Kernel.Bind<IHandler, INotificationsHandler>().To<NotificationsHandler>().InSingletonScope();
        }
    }
}