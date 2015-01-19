namespace Omnipaste.NotificationList
{
    using Omnipaste.Models;
    using Omnipaste.Notification;

    public interface INotificationViewModelFactory
    {
        INotificationViewModel Create(ClippingModel clipping);

        INotificationViewModel Create(Call call);

        INotificationViewModel Create(Message message);
    }
}