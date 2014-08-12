namespace Omnipaste.Notification
{
    using Caliburn.Micro;

    public interface INotificationViewModel : IScreen
    {
        #region Public Properties

        string Message { get; set; }

        string Title { get; }

        #endregion
    }
}