namespace Omnipaste.WorkspaceDetails.Conversation
{
    using System.Collections.ObjectModel;
    using Omnipaste.Presenters;

    public interface IConversationHeaderViewModel : IWorkspaceDetailsHeaderViewModel
    {
        ConversationHeaderStateEnum State { get; set; }

        ObservableCollection<ContactInfoPresenter> Recipients { get; set; }
    }
}