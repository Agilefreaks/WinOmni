namespace Omnipaste.WorkspaceDetails
{
    using Omnipaste.Presenters;

    public interface IWorkspaceDetailsViewModelFactory
    {
        IWorkspaceDetailsViewModel Create(ActivityPresenter activity);
        
        IWorkspaceDetailsViewModel Create(ContactInfoPresenter activity);
    }
}