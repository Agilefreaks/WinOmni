namespace Omnipaste.Event
{
    using Events.Models;

    public interface IEventViewModel : IDetailsViewModel<Event>
    {
        #region Public Properties

        EventTypeEnum Type { get; }

        #endregion

        void SendSms();
    }
}