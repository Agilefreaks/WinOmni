namespace Omnipaste.ClippingList
{
    using Caliburn.Micro;
    using Ninject;

    public class MasterClippingListViewModel : Screen, IMasterClippingListViewModel
    {
        [Inject]
        public IAllClippingListViewModel AllClippingListViewModel { get; set; }

        [Inject]
        public ICloudClippingListViewModel CloudClippingListViewModel { get; set; }

        [Inject]
        public ILocalClippingListViewModel LocalClippingListViewModel { get; set; }
    }
}