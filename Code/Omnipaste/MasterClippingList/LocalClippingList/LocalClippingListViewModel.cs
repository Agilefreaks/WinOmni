namespace Omnipaste.MasterClippingList.LocalClippingList
{
    using Clipboard.Handlers;
    using Omnipaste.MasterClippingList.ClippingList;
    using OmniUI.Attributes;

    [UseView("Omnipaste.MasterClippingList.ClippingList.ClippingListView", IsFullyQualifiedName = true)]
    public class LocalClippingListViewModel : ClippingListViewModelBase, ILocalClippingListViewModel
    {
        public LocalClippingListViewModel(ILocalClipboardHandler localClipboardHandler)
            : base(localClipboardHandler.Clippings)
        {
        }
    }
}