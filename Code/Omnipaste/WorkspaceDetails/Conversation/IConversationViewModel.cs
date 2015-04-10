namespace Omnipaste.WorkspaceDetails.Conversation
{
    using System.Collections.ObjectModel;
    using Omnipaste.Framework.Models;

    public interface IConversationViewModel : IWorkspaceDetailsViewModel
    {
        ObservableCollection<ContactModel> Recipients { get; set; }
    }
}