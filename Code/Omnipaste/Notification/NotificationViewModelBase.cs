namespace Omnipaste.Notification
{
    using Caliburn.Micro;

    public abstract class NotificationViewModelBase: Screen, INotificationViewModel
    {
        #region Public Properties

        public string Message { get; set; }

        public abstract string Title { get; }

        #endregion
    }
}