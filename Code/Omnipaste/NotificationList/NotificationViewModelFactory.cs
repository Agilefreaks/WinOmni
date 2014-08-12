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
            INotificationViewModel result = null;
            switch (@event.Type)
            {
                case EventTypeEnum.IncomingCallEvent:
                    {
                        var incomingCallNotificationViewModel = Kernel.Get<IIncomingCallNotificationViewModel>();
                        incomingCallNotificationViewModel.PhoneNumber = @event.phone_number;

                        result = incomingCallNotificationViewModel;
                    }
                    break;
                case EventTypeEnum.IncomingSmsEvent:
                    {
                        var incomingSmsNotificationViewModel = Kernel.Get<IIncomingSmsNotificationViewModel>();
                        incomingSmsNotificationViewModel.PhoneNumber = @event.phone_number;

                        result = incomingSmsNotificationViewModel;
                    }
                    break;
            }

            return result;
        }
    }
}