namespace Omnipaste.Notification.ClippingNotification
{
    using Omnipaste.Properties;

    public class ClippingNotificationViewModel : NotificationViewModelBase, IClippingNotificationViewModel
    {
        #region Public Properties

        public override string Title
        {
            get
            {
                return Resources.ClippingNotificationTitle;
            }
        }

        public override NotificationTypeEnum Type
        {
            get
            {
                return NotificationTypeEnum.Clipping;
            }
        }

        #endregion
    }
}