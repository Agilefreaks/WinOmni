namespace Omnipaste.MasterClippingList.LocalClippingList
{
    using Clipboard.Handlers;
    using Omnipaste.MasterClippingList.ClippingList;
    using OmniUI.Attributes;

    [UseView(typeof(ClippingListView))]
    public class LocalClippingListViewModel : ClippingListViewModelBase, ILocalClippingListViewModel
    {
        public LocalClippingListViewModel(ILocalClipboardHandler localClipboardHandler)
            : base(localClipboardHandler.Clippings)
        {
        }
    }
}