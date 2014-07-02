namespace Omnipaste.Notification
{
    using Caliburn.Micro;

    public interface INotificationViewModel : IScreen
    {
        #region Public Properties

        string Message { get; }

        string Title { get; }

        #endregion
    }
}