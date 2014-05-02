using Ninject.Modules;
using OmniCommon.Interfaces;

namespace Notifications
{
    public class NotificationsModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IOmniMessageHandler>().To<NotificationsHandler>();
        }
    }
}