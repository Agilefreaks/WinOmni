namespace Omnipaste.MasterClippingList.CloudClippingList
{
    using Clipboard.Handlers;
    using Omnipaste.Framework.Attributes;

    [UseView("Omnipaste.MasterClippingList.ClippingListView", IsFullyQualifiedName = true)]
    public class CloudClippingListViewModel : ClippingListViewModelBase, ICloudClippingListViewModel
    {
        public CloudClippingListViewModel(IOmniClipboardHandler entityObservable)
            : base(entityObservable)
        {
        }
    }
}