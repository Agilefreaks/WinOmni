namespace Omnipaste.GroupMessage.GroupMessageDetails
{
    using System.Collections.ObjectModel;
    using Omnipaste.Presenters;
    using Omnipaste.WorkspaceDetails;

    public interface IGroupMessageDetailsViewModel : IWorkspaceDetailsViewModel
    {
        ObservableCollection<ContactInfoPresenter> Recipients { get; set; }
    }
}