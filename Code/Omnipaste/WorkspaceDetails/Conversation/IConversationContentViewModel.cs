namespace Omnipaste.WorkspaceDetails.Conversation
{
    using Caliburn.Micro;
    using Omnipaste.Models;

    public interface IConversationContentViewModel : IConductor, IActivate, IDeactivate
    {
        ContactModel Model { get; set; }

        void RefreshConversation();
    }
}