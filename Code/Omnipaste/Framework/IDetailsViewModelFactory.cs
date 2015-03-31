namespace Omnipaste.WorkspaceDetails
{
    using System.Collections.Generic;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;
    using OmniUI.Details;

    public interface IDetailsViewModelFactory
    {
        IDetailsViewModelWithHeader Create(ActivityModel activityModel);

        IDetailsViewModelWithHeader Create(ClippingEntity clippingEntity);

        IDetailsViewModelWithHeader Create(IEnumerable<ContactModel> contactModelList);

        IDetailsViewModelWithHeader Create(UpdateEntity updateEntity);

        IDetailsViewModelWithHeader Create(ContactEntity contactEntity);
    }
}