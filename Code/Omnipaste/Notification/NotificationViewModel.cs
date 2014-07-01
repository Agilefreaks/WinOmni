namespace Omnipaste.Notification
{
    using System;
    using Caliburn.Micro;

    public class NotificationViewModel : Screen, INotificationViewModel
    {
        #region Public Properties

        public String Message { get; set; }

        public String Title { get; set; }

        public NotificationViewModelTypeEnum Type { get; set; }

        #endregion
    }
}