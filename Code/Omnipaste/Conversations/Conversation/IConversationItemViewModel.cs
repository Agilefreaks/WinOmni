namespace Omnipaste.Conversations.Conversation
{
    using System;
    using OmniUI.Details;

    public interface IConversationItemViewModel : IDetailsViewModel
    {
        DateTime Time { get; }
    }
}