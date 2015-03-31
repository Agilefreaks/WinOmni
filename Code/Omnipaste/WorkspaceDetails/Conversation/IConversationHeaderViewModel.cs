namespace Omnipaste.WorkspaceDetails.Conversation
{
    using System.Collections.ObjectModel;
    using Omnipaste.Models;

    public interface IConversationHeaderViewModel : IWorkspaceDetailsHeaderViewModel
    {
        ConversationHeaderStateEnum State { get; set; }

        ObservableCollection<ContactModel> Recipients { get; set; }
    }
}