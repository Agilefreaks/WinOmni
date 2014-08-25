namespace Omnipaste.Notification.HyperlinkNotification
{
    using System.ComponentModel;
    using System.Diagnostics;

    public class HyperlinkNotificationViewModel : NotificationViewModelBase, IHyperlinkNotificationViewModel
    {
        private bool _canOpenLink = true;

        #region Public Properties

        public override string Title
        {
            get
            {
                return "Incoming Link";
            }
        }

        public bool CanOpenLink
        {
            get
            {
                return _canOpenLink;
            }
            set
            {
                if (value.Equals(_canOpenLink)) return;
                _canOpenLink = value;
                NotifyOfPropertyChange(() => CanOpenLink);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void OpenLink()
        {
            CanOpenLink = false;
            try
            {
                Process.Start(Message);
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