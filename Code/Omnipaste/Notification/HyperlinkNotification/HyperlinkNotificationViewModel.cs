namespace Omnipaste.Notification.HyperlinkNotification
{
    using System.ComponentModel;
    using System.Diagnostics;
    using Omnipaste.Notification.Models;

    public class HyperlinkNotificationViewModel : NotificationViewModelBase<HyperlinkNotification>, IHyperlinkNotificationViewModel
    {
        public void OpenLink()
        {
            try
            {
                Process.Start(Message);
            }
            catch (Win32Exception)
            {
                // Looks like there is no way for us to act on this
            }
        }
    }
}