namespace Omnipaste.Notification
{
    using System;
    using Caliburn.Micro;
    using Notifications.Models;

    public class NotificationViewModel : Screen, INotificationViewModel
    {
        #region Constructors and Destructors

        public NotificationViewModel(Notification model)
        {
            Model = model;
        }

        #endregion

        #region Public Properties

        public String Message
        {
            get
            {
                return Model.Message;
            }
        }

        public Notification Model { get; private set; }

        public String Title
        {
            get
            {
                return string.Concat("Incoming call from ", Model.phone_number);
            }
        }

        #endregion
    }
}