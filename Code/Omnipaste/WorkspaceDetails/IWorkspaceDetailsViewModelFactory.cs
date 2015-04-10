namespace Omnipaste.WorkspaceDetails
{
    using System.Collections.Generic;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;

    public interface IWorkspaceDetailsViewModelFactory
    {
        IWorkspaceDetailsViewModel Create(ActivityModel activityModel);

        IWorkspaceDetailsViewModel Create(ClippingEntity clippingEntity);

        IWorkspaceDetailsViewModel Create(IEnumerable<ContactModel> contactModelList);

        IWorkspaceDetailsViewModel Create(UpdateEntity updateEntity);

        IWorkspaceDetailsViewModel Create(ContactEntity contactEntity);
    }
}