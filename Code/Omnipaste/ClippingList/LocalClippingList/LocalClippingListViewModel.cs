namespace Omnipaste.ClippingList.LocalClippingList
{
    using Clipboard.Handlers;
    using Omnipaste.Framework.Attributes;

    [UseView("Omnipaste.ClippingList.ClippingListView", IsFullyQualifiedName = true)]
    public class LocalClippingListViewModel : ClippingListViewModelBase, ILocalClippingListViewModel
    {
        public LocalClippingListViewModel(ILocalClipboardHandler localClipboardHandler)
            : base(localClipboardHandler)
        {
        }
    }
}