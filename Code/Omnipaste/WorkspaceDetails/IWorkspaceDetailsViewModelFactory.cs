namespace Omnipaste.WorkspaceDetails
{
    using Omnipaste.Presenters;

    public interface IWorkspaceDetailsViewModelFactory
    {
        IWorkspaceDetailsViewModel Create(ActivityPresenter activityPresenter);
        
        IWorkspaceDetailsViewModel Create(IContactInfoPresenter contactInfoPresenter);

        IWorkspaceDetailsViewModel Create(ClippingPresenter clippingPresenter);
    }
}