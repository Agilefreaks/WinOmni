namespace Omnipaste.Notifications.NotificationList.Notification.HyperlinkNotification
{
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Helpers;
    using Omnipaste.Notifications.NotificationList.Notification.ClippingNotification;
    using Omnipaste.Properties;
    using OmniUI.Framework;

    public class HyperlinkNotificationViewModel : ClippingNotificationViewModel, IHyperlinkNotificationViewModel
    {
        #region Fields

        private bool _canOpenLink = true;

        #endregion

        #region Public Properties

        public bool CanOpenLink
        {
            get
            {
                return _canOpenLink;
            }
            set
            {
                if (value.Equals(_canOpenLink))
                {
                    return;
                }
                _canOpenLink = value;
                NotifyOfPropertyChange();
            }
        }

        public override ClippingEntity Resource
        {
            get
            {
                return base.Resource;
            }
            set
            {
                var oldResource = base.Resource;
                base.Resource = value;
                if (oldResource != value)
                {
                    NotifyOfPropertyChange(() => Uri);
                }
            }
        }

        public override string Title
        {
            get
            {
                return Resources.HyperlinkNotificationTitle;
            }
        }

        public override string Line2
        {
            get
            {
                return string.Empty;
            }
        }

        public override NotificationTypeEnum Type
        {
            get
            {
                return NotificationTypeEnum.Link;
            }
        }

        public string Uri
        {
            get
            {
                return Resource.Content;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void OpenLink()
        {
            CanOpenLink = false;
            ExternalProcessHelper.Start(Uri);
            Dismiss();
        }

        #endregion
    }
}