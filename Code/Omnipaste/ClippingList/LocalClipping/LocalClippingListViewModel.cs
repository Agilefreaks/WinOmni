namespace Omnipaste.ClippingList
{
    using Clipboard.Handlers;

    public class LocalClippingListViewModel : ClippingListViewModelBase, ILocalClippingListViewModel
    {
        public LocalClippingListViewModel(ILocalClipboardHandler localClipboardHandler)
            : base(localClipboardHandler)
        {
        }
    }
}