namespace Omnipaste.WorkspaceDetails.Conversation
{
    using System.Collections.ObjectModel;
    using Omnipaste.Models;
    using Omnipaste.SMSComposer;

    public interface IConversationContainerViewModel : IWorkspaceDetailsContentViewModel
    {
        ObservableCollection<ContactModel> Recipients { get; set; }

        IConversationContentViewModel ConversationContentViewModel { get; set; }

        ISMSComposerViewModel SMSComposer { get; set; }
    }
}