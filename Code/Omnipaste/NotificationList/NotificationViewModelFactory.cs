namespace Omnipaste.NotificationList
{
    using Events.Models;
    using Ninject;
    using Omnipaste.Notification;
    using Clipboard.Models;
    using Omnipaste.Notification.ClippingNotification;
    using Omnipaste.Notification.HyperlinkNotification;
    using Omnipaste.Notification.IncomingCallNotification;
    using Omnipaste.Notification.IncomingSmsNotification;

    public class NotificationViewModelFactory : INotificationViewModelFactory
    {
        [Inject]
        public IKernel Kernel { get; set; }

        public INotificationViewModel Create(Clipping clipping)
        {
            INotificationViewModel result;

            if (clipping.Type == Clipping.ClippingTypeEnum.Url)
            {
                result = Kernel.Get<IHyperlinkNotificationViewModel>();
            }
            else
            {
                result = Kernel.Get<IClippingNotificationViewModel>();
            }

            result.Message = clipping.Content;

            return result;
        }

        public INotificationViewModel Create(Event @event)
        {
            IEventNotificationViewModel result;

            if (@event.Type == EventTypeEnum.IncomingCallEvent)
            {
                result = Kernel.Get<IIncomingCallNotificationViewModel>();
            }
            else
            {
                result = Kernel.Get<IIncomingSmsNotificationViewModel>();
            }

            result.PhoneNumber = @event.PhoneNumber;
            result.Message = @event.Content;

            return result;
        }
    }
}