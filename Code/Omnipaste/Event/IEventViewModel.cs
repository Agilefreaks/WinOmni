namespace Omnipaste.Event
{
    using Events.Models;
    using Omnipaste.DetailsViewModel;
    using OmniUI.Details;

    public interface IEventViewModel : IDetailsViewModel<Event>
    {
        #region Public Properties

        EventTypeEnum Type { get; }

        string Title { get; }

        #endregion

        #region Public Methods and Operators

        void CallBack();

        void SendSms();

        #endregion
    }
}