namespace Omnipaste.NotificationList
{
    using Events.Models;
    using Omnipaste.Notification;
    using Clipboard.Models;

    public class NotificationViewModelBuilder
    {
        public static NotificationViewModel Build(Clipping clipping)
        {
            var notificationViewModel = new NotificationViewModel
                                        {
                                            Title = "New clipping",
                                            Message = clipping.content,
                                            Type =
                                                clipping.type == "web_site"
                                                    ? NotificationViewModelTypeEnum.Hyperlink
                                                    : NotificationViewModelTypeEnum.Clipping
                                        };

            return notificationViewModel;
        }

        public static NotificationViewModel Build(Event @event)
        {

            var notificationViewModel = new NotificationViewModel
                                        {
                                            Title = string.Concat("Incoming call from ", @event.phone_number),
                                            Type = NotificationViewModelTypeEnum.IncomingCall
                                        };

            return notificationViewModel;
        }
    }
}