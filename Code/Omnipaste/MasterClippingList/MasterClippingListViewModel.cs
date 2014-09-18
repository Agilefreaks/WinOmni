namespace Omnipaste.MasterClippingList
{
    using Caliburn.Micro;
    using Ninject;
    using Omnipaste.MasterClippingList.AllClippingList;
    using Omnipaste.MasterClippingList.CloudClippingList;
    using Omnipaste.MasterClippingList.LocalClippingList;
    using Omnipaste.Properties;

    public sealed class MasterClippingListViewModel : Screen, IMasterClippingListViewModel
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
    }
}