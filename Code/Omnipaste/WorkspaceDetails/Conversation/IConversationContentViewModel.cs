namespace Omnipaste.WorkspaceDetails.Conversation
{
    using Caliburn.Micro;
    using Omnipaste.Presenters;

    public interface IConversationContentViewModel : IConductor, IActivate, IDeactivate
    {
        ContactInfoPresenter Model { get; set; }

        void RefreshConversation();
    }
}