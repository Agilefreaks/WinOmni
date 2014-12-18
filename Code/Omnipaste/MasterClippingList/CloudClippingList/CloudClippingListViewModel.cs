namespace Omnipaste.MasterClippingList.CloudClippingList
{
    using Clipboard.Handlers;
    using Omnipaste.MasterClippingList.ClippingList;
    using OmniUI.Attributes;

    [UseView(typeof(ClippingListView))]
    public class CloudClippingListViewModel : ClippingListViewModelBase, ICloudClippingListViewModel
    {
        public CloudClippingListViewModel(IOmniClipboardHandler entityObservable)
            : base(entityObservable.Clippings)
        {
        }
    }
}