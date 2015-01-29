namespace Omnipaste.Conversation
{
    using Omnipaste.Presenters;

    public interface IConversationInfoViewModelFactory
    {
        IConversationInfoViewModel Create(ConversationPresenter presenter);
    }
}