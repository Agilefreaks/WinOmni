﻿namespace Omnipaste.Notifications.NotificationList.Notification.ClippingNotification
{
    using Omnipaste.Framework.Entities;
    using Omnipaste.Properties;
    using OmniUI.Framework;

    public class ClippingNotificationViewModel : ResourceBasedNotificationViewModel<ClippingEntity>, IClippingNotificationViewModel
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