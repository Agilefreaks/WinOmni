namespace Omnipaste.NotificationList
{
    using Omnipaste.Models;
    using Omnipaste.Notification;

    public interface INotificationViewModelFactory
    {
        INotificationViewModel Create(ClippingModel clipping);

        INotificationViewModel Create(PhoneCall phoneCall);

        INotificationViewModel Create(SmsMessage smsMessage);
    }
}