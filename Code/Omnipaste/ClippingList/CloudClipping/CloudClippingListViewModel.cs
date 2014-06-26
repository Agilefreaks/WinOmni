namespace Omnipaste.ClippingList
{
    using Clipboard.Handlers;

    public class CloudClippingListViewModel : ClippingListViewModelBase, ICloudClippingListViewModel
    {
        public CloudClippingListViewModel(IOmniClipboardHandler clippingsObservable)
            : base(clippingsObservable)
        {
        }
    }
}