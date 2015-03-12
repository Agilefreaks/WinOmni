namespace Omnipaste.WorkspaceDetails.Conversation.Message
{
    using Omnipaste.Presenters;
    using OmniUI.Details;

    public interface IMessageViewModel : IConversationItemViewModel, IDetailsViewModel<SmsMessagePresenter>
    {
    }
}