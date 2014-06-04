namespace Omnipaste.Notification
{
    using System;
    using Caliburn.Micro;
    using Notifications.Models;

    public interface INotificationViewModel : IScreen
    {
        #region Public Properties

        String Message { get; }

        Notification Model { get; }

        String Title { get; }

        #endregion
    }
}