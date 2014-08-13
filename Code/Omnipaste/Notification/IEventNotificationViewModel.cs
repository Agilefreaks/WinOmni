namespace Omnipaste.Notification
{
    public interface IEventNotificationViewModel : INotificationViewModel
    {
        string PhoneNumber { get; set; }
    }
}