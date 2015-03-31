namespace Omnipaste.WorkspaceDetails.Conversation
{
    using System.Collections.ObjectModel;
    using Omnipaste.Presenters;
    using Omnipaste.SMSComposer;

    public interface IConversationContainerViewModel : IWorkspaceDetailsContentViewModel
    {
        ObservableCollection<ContactInfoPresenter> Recipients { get; set; }

        IConversationContentViewModel ConversationContentViewModel { get; set; }

        ISMSComposerViewModel SMSComposer { get; set; }
    }
}