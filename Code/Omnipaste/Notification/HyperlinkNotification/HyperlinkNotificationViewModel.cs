namespace Omnipaste.Notification.HyperlinkNotification
{
    using System.ComponentModel;
    using System.Diagnostics;

    public class HyperlinkNotificationViewModel : NotificationViewModelBase, IHyperlinkNotificationViewModel
    {
        #region Public Properties

        public override string Title
        {
            get
            {
                return "Incoming Link";
            }
        }

        #endregion

        #region Public Methods and Operators

        public void OpenLink()
        {
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