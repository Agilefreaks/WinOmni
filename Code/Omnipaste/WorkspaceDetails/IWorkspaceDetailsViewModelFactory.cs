namespace Omnipaste.WorkspaceDetails
{
    using System.Collections.Generic;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services;

    public interface IWorkspaceDetailsViewModelFactory
    {
        IWorkspaceDetailsViewModel Create(ActivityPresenter activityPresenter);

        IWorkspaceDetailsViewModel Create(ClippingModel clippingModel);

        IWorkspaceDetailsViewModel Create(IEnumerable<ContactInfoPresenter> contactInfoPresenterList);

        IWorkspaceDetailsViewModel Create(UpdateInfo updateInfo);

        IWorkspaceDetailsViewModel Create(ContactInfo contactInfo);
    }
}