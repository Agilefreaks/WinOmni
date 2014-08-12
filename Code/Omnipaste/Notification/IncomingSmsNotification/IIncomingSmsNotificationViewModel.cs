namespace Omnipaste.Notification.IncomingSmsNotification
{
    public interface IIncomingSmsNotificationViewModel : INotificationViewModel
    {
        string PhoneNumber { get; set; }
    }
}