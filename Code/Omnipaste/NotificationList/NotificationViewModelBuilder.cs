namespace Omnipaste.NotificationList
{
    using Notifications.Models;
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
                                            Type = NotificationViewModelTypeEnum.Clipping
                                        };

            return notificationViewModel;
        }

        public static NotificationViewModel Build(Notification notification)
        {
            var notificationViewModel = new NotificationViewModel
                                        {
                                            Title = string.Concat("Incoming call from ", notification.phone_number),
                                            Type = NotificationViewModelTypeEnum.IncomingCall
                                        };

            return notificationViewModel;
        }
    }
}