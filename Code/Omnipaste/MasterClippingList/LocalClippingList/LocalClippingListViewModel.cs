namespace Omnipaste.MasterClippingList.LocalClippingList
{
    using Clipboard.Models;
    using Omnipaste.MasterClippingList.ClippingList;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using OmniUI.Attributes;

    [UseView(typeof(ClippingListView))]
    public class LocalClippingListViewModel : ClippingListViewModelBase, ILocalClippingListViewModel
    {
        public LocalClippingListViewModel(IClippingRepository clippingRepository)
            : base(clippingRepository)
        {
        }

        public override bool CanHandle(ClippingModel clipping)
        {
            return clipping.Source == Clipping.ClippingSourceEnum.Local;
        }
    }
}