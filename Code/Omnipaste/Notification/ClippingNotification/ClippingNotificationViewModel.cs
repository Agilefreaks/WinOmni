namespace Omnipaste.Notification.ClippingNotification
{
    using Clipboard.Models;
    using Omnipaste.Properties;

    public class ClippingNotificationViewModel : ResourceBasedNotificationViewModel<Clipping>, IClippingNotificationViewModel
    {
        #region Public Properties

        public override string Line1
        {
            get
            {
                return string.Empty;
            }
        }

        public override string Line2
        {
            get
            {
                return Resource.Content;
            }
        }

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

        public override object Identifier
        {
            get
            {
                return Resource.UniqueId;
            }
        }

        #endregion
    }
}