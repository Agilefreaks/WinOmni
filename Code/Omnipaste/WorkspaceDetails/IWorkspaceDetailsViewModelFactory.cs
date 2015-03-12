namespace Omnipaste.WorkspaceDetails
{
    using System.Collections.Generic;
    using Omnipaste.Presenters;

    public interface IWorkspaceDetailsViewModelFactory
    {
        IWorkspaceDetailsViewModel Create(ActivityPresenter activityPresenter);
        
        IWorkspaceDetailsViewModel Create(IContactInfoPresenter contactInfoPresenter);

        IWorkspaceDetailsViewModel Create(ClippingPresenter clippingPresenter);

        IWorkspaceDetailsViewModel Create(IEnumerable<ContactInfoPresenter> contactInfoPresenterList);
    }
}