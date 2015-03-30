namespace Omnipaste.WorkspaceDetails.Conversation
{
    using System.Collections.ObjectModel;
    using Omnipaste.Presenters;

    public interface IConversationContainerViewModel : IWorkspaceDetailsContentViewModel
    {
        ObservableCollection<ContactInfoPresenter> Recipients { get; set; }
    }
}