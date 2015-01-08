namespace Omnipaste.MasterClippingList
{
    using Caliburn.Micro;
    using Ninject;
    using Omnipaste.MasterClippingList.AllClippingList;
    using Omnipaste.MasterClippingList.CloudClippingList;
    using Omnipaste.MasterClippingList.LocalClippingList;
    using Omnipaste.Properties;

    public sealed class MasterClippingListViewModel : Conductor<IScreen>.Collection.AllActive, IMasterClippingListViewModel
    {
        [Inject]
        public IAllClippingListViewModel AllClippingListViewModel { get; set; }

        [Inject]
        public ICloudClippingListViewModel CloudClippingListViewModel { get; set; }

        [Inject]
        public ILocalClippingListViewModel LocalClippingListViewModel { get; set; }

        public MasterClippingListViewModel()
        {
            DisplayName = Resources.MasterClippingListDisplayName;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            ActivateItem(AllClippingListViewModel);
            ActivateItem(CloudClippingListViewModel);
            ActivateItem(LocalClippingListViewModel);
        }

        protected override void OnDeactivate(bool close)
        {
            DeactivateItem(AllClippingListViewModel, close);
            DeactivateItem(CloudClippingListViewModel, close);
            DeactivateItem(LocalClippingListViewModel, close);
            base.OnActivate();
        }
    }
}