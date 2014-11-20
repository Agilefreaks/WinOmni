namespace Omnipaste.Notification.HyperlinkNotification
{
    using System.ComponentModel;
    using System.Diagnostics;
    using Omnipaste.Properties;

    public class HyperlinkNotificationViewModel : NotificationViewModelBase, IHyperlinkNotificationViewModel
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

        public override string Title
        {
            get
            {
                return Resources.HyperlinkNotificationTitle;
            }
        }

        public override NotificationTypeEnum Type
        {
            get
            {
                return NotificationTypeEnum.Link;
            }
        }

        public override string Line2
        {
            get
            {
                return string.Empty;
            }
            set
            {
                Uri = value;
                NotifyOfPropertyChange(() => Uri);
            }
        }

        public string Uri { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void OpenLink()
        {
            CanOpenLink = false;
            try
            {
                Process.Start(Uri);
                Dismiss();
            }
            catch (Win32Exception)
            {
                // Looks like there is no way for us to act on this
            }
        }

        #endregion
    }
}