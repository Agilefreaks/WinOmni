namespace Omnipaste.Event
{
    using Omnipaste.DetailsViewModel;
    using OmniUI.Details;

    public interface IEventViewModel : IDetailsViewModel<IConversationItem>
    {
        #region Public Properties

        string Title { get; }

        #endregion

        #region Public Methods and Operators

        void CallBack();

        void SendSms();

        #endregion
    }
}