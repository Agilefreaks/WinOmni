namespace Omnipaste.NotificationList.Notification
{
    using Caliburn.Micro;

    public interface INotificationViewModel : IScreen
    {
        #region Public Properties

        object Identifier { get; }

        string Line1 { get; }

        string Line2 { get; }

        string Title { get; }

        #endregion

        #region Public Methods and Operators
        
        void Dismiss();
        
        #endregion
    }
}