namespace Omnipaste.Conversations.Conversation.Message
{
    using Omnipaste.Framework.Models;
    using OmniUI.Details;

    public interface IMessageViewModel : IConversationItemViewModel, IDetailsViewModel<SmsMessageModel>
    {
    }
}