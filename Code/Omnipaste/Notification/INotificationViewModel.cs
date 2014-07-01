namespace Omnipaste.Notification
{
    using System;
    using Caliburn.Micro;

    public interface INotificationViewModel : IScreen
    {
        #region Public Properties

        String Message { get; set; }

        String Title { get; set; }

        NotificationViewModelTypeEnum Type { get; set; }

        #endregion
    }
}