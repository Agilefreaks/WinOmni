namespace Omnipaste.ClippingList
{
    using Caliburn.Micro;
    using Ninject;
    using Omnipaste.ClippingList.AllClippingList;
    using Omnipaste.ClippingList.CloudClippingList;
    using Omnipaste.ClippingList.LocalClippingList;

    public class MasterClippingListViewModel : Screen, IMasterClippingListViewModel
    {
        [Inject]
        public IAllClippingListViewModel AllClippingListViewModel { get; set; }

        [Inject]
        public ICloudClippingListViewModel CloudClippingListViewModel { get; set; }

        [Inject]
        public ILocalClippingListViewModel LocalClippingListViewModel { get; set; }

        public MasterClippingListViewModel()
        {
            DisplayName = "Clippings";
        }
    }
}