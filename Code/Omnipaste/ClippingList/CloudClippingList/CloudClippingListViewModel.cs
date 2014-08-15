namespace Omnipaste.ClippingList.CloudClippingList
{
    using Clipboard.Handlers;
    using Omnipaste.Framework.Attributes;

    [UseView("Omnipaste.ClippingList.ClippingListView", IsFullyQualifiedName = true)]
    public class CloudClippingListViewModel : ClippingListViewModelBase, ICloudClippingListViewModel
    {
        public CloudClippingListViewModel(IOmniClipboardHandler entityObservable)
            : base(entityObservable)
        {
        }
    }
}