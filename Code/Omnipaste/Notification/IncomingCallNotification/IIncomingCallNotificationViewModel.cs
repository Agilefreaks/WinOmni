namespace Omnipaste.Notification.IncomingCallNotification
{
    public interface IIncomingCallNotificationViewModel : INotificationViewModel
    {
        string PhoneNumber { get; set; }
    }
}