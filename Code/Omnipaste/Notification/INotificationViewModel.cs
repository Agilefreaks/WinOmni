namespace Omnipaste.Notification
{
    using Caliburn.Micro;

    public interface INotificationViewModel : IScreen
    {
        #region Public Properties

        string Line1 { get; }

        string Line2 { get; }

        string Title { get; }

        #endregion
    }
}