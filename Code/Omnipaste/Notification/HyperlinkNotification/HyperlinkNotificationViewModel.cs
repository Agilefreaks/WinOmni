namespace Omnipaste.Notification.HyperlinkNotification
{
    using System.Diagnostics;
    using Omnipaste.Notification.Models;

    public class HyperlinkNotificationViewModel : NotificationViewModelBase<HyperlinkNotification>, IHyperlinkNotificationViewModel
    {
        public void OpenLink()
        {
            Process.Start(Message);
        }
    }
}