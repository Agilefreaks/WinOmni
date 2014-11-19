namespace Omnipaste.Notification
{
    using Caliburn.Micro;

    public interface INotificationViewModel : IScreen
    {
        #region Public Properties

        string Line1 { get; set; }

        string Line2 { get; set; }

        string Title { get; }

        #endregion
    }
}