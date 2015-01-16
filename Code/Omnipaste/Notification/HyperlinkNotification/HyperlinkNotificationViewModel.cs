﻿namespace Omnipaste.Notification.HyperlinkNotification
{
    using Omnipaste.Helpers;
    using Omnipaste.Models;
    using Omnipaste.Notification.ClippingNotification;
    using Omnipaste.Properties;

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
                NotifyOfPropertyChange(() => CanOpenLink);
            }
        }

        public override ClippingModel Resource
        {
            get
            {
                return base.Resource;
            }
            set
            {
                base.Resource = value;
                NotifyOfPropertyChange(() => Uri);
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