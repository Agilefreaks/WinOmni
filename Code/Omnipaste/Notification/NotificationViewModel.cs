namespace Omnipaste.Notification
{
    using System;
    using System.Diagnostics;
    using Caliburn.Micro;
    using Omnipaste.Framework;

    public class NotificationViewModel : Screen, INotificationViewModel
    {
        #region Public Properties

        public bool CanOpenUrl
        {
            get
            {
                return Type == NotificationViewModelTypeEnum.Hyperlink;
            }
        }

        public String Message { get; set; }

        public String Title { get; set; }

        public NotificationViewModelTypeEnum Type { get; set; }

        #endregion

        #region Public Methods and Operators

        public void OpenLink()
        {
            Process.Start(Message);
        }

        #endregion
    }
}