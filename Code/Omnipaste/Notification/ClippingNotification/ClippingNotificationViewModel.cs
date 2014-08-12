namespace Omnipaste.Notification.ClippingNotification
{
    public class ClippingNotificationViewModel : NotificationViewModelBase, IClippingNotificationViewModel
    {
        #region Public Properties

        public override string Title
        {
            get
            {
                return "New clipping";
            }
        }

        #endregion
    }
}