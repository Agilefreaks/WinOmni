using Caliburn.Micro;
using OmniCommon.Models;

namespace Omnipaste.Services
{
    public class NotificationService : INotificationService, IHandle<OmniMessage>
    {
        public void Handle(OmniMessage message)
        {
            
        }
    }
}