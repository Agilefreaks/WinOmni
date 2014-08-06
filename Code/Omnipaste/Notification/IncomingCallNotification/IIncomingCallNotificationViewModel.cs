namespace Omnipaste.Notification.IncomingCallNotification
{
    using Omnipaste.Notification.Models;
    public interface IIncomingCallNotificationViewModel : INotificationViewModel
    {
        IncomingCallNotification Model { get; set; }
    }
}