namespace Omnipaste.MasterClippingList.LocalClippingList
{
    using Clipboard.Handlers;
    using Omnipaste.Framework.Attributes;
    using Omnipaste.MasterClippingList.ClippingList;

    [UseView("Omnipaste.MasterClippingList.ClippingList.ClippingListView", IsFullyQualifiedName = true)]
    public class LocalClippingListViewModel : ClippingListViewModelBase, ILocalClippingListViewModel
    {
        public LocalClippingListViewModel(ILocalClipboardHandler localClipboardHandler)
            : base(localClipboardHandler)
        {
        }
    }
}