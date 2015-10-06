namespace Omnipaste.Framework
{
    using System.Collections.ObjectModel;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Models;
    using OmniUI.Details;

    public interface IDetailsViewModelFactory
    {
        IDetailsViewModelWithHeader Create(ActivityModel activityModel);

        IDetailsViewModelWithHeader Create(ClippingEntity clippingEntity);

        IDetailsViewModelWithHeader Create(ObservableCollection<ContactModel> contactModelList);

        IDetailsViewModelWithHeader Create(UpdateEntity updateEntity);

        IDetailsViewModelWithHeader Create(ContactEntity contactEntity);
    }
}