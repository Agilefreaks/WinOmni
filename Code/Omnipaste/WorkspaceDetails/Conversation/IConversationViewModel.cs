namespace Omnipaste.WorkspaceDetails.Conversation
{
    using System.Collections.ObjectModel;
    using Omnipaste.Presenters;

    public interface IConversationViewModel : IWorkspaceDetailsViewModel
    {
        ObservableCollection<ContactInfoPresenter> Recipients { get; set; }
    }
}