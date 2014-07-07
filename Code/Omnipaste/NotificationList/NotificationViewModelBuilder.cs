namespace Omnipaste.NotificationList
{
    using Events.Models;
    using Omnipaste.Notification;
    using Clipboard.Models;
    using Omnipaste.Notification.ClippingNotification;
    using Omnipaste.Notification.HyperlinkNotification;
    using Omnipaste.Notification.IncomingCallNotification;
    using Omnipaste.Notification.Models;

    public class NotificationViewModelBuilder
    {
        public static INotificationViewModel Build(Clipping clipping)
        {
            INotificationViewModel viewModel;

            if (clipping.Type == Clipping.ClippingTypeEnum.WebSite)
            {
                var model = new HyperlinkNotification { Title = "Incoming Link", Message = clipping.Content };
                viewModel = new HyperlinkNotificationViewModel { Model = model };
            }
            else
            {
                var model = new ClippingNotification { Title = "New clipping", Message = clipping.Content };
                viewModel = new ClippingNotificationViewModel { Model = model };
            }

            return viewModel;
        }

        public static INotificationViewModel Build(Event @event)
        {
            var model = new IncomingCallNotification
                        {
                            Title = string.Concat("Incoming call from ", @event.phone_number)
                        };
            return new IncomingCallNotificationViewModel { Model = model };
        }
    }
}