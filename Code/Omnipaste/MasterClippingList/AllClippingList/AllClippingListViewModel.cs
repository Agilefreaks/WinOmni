namespace Omnipaste.MasterClippingList.AllClippingList
{
    using Omnipaste.MasterClippingList.ClippingList;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using OmniUI.Attributes;

    [UseView(typeof(ClippingListView))]
    public class AllClippingListViewModel : ClippingListViewModelBase, IAllClippingListViewModel
    {
        public AllClippingListViewModel(IClippingRepository clippingRepository)
            : base(clippingRepository)
        {
        }

        public override bool CanHandle(ClippingModel clipping)
        {
            return true;
        }
    }
}