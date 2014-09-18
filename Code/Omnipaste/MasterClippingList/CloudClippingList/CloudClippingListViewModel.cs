namespace Omnipaste.MasterClippingList.CloudClippingList
{
    using Clipboard.Handlers;
    using Omnipaste.Framework.Attributes;
    using Omnipaste.MasterClippingList.ClippingList;

    [UseView("Omnipaste.MasterClippingList.ClippingList.ClippingListView", IsFullyQualifiedName = true)]
    public class CloudClippingListViewModel : ClippingListViewModelBase, ICloudClippingListViewModel
    {
        public CloudClippingListViewModel(IOmniClipboardHandler entityObservable)
            : base(entityObservable)
        {
        }
    }
}