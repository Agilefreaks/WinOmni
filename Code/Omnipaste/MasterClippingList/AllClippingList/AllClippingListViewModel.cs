namespace Omnipaste.MasterClippingList.AllClippingList
{
    using Clipboard.Handlers;
    using Omnipaste.MasterClippingList.ClippingList;
    using OmniUI.Attributes;

    [UseView(typeof(ClippingListView))]
    public class AllClippingListViewModel : ClippingListViewModelBase, IAllClippingListViewModel
    {
        public AllClippingListViewModel(IClipboardHandler entityObservable)
            : base(entityObservable)
        {
        }
    }
}