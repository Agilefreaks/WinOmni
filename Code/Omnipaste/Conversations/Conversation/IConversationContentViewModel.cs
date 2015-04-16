namespace Omnipaste.Conversations.Conversation
{
    using Caliburn.Micro;
    using Omnipaste.Framework.Models;

    public interface IConversationContentViewModel : IConductor, IActivate, IDeactivate
    {
        ContactModel Model { get; set; }

        void RefreshConversation();
    }
}