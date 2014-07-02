namespace Omnipaste.Notification
{
    using System;
    using Caliburn.Micro;
    using Omnipaste.Notification.Models;

    public abstract class NotificationViewModelBase<TModel> : Screen, INotificationViewModel
        where TModel : NotificationBase
    {
        #region Public Properties

        public TModel Model { get; set; }

        public String Message
        {
            get
            {
                return Model.Message;
            }
        }

        public String Title
        {
            get
            {
                return Model.Title;
            }
        }

        #endregion
    }
}