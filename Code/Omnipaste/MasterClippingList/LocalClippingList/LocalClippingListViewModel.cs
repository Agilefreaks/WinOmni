namespace Omnipaste.MasterClippingList.LocalClippingList
{
    using Clipboard.Handlers;
    using Omnipaste.Framework.Attributes;

    [UseView("Omnipaste.MasterClippingList.ClippingListView", IsFullyQualifiedName = true)]
    public class LocalClippingListViewModel : ClippingListViewModelBase, ILocalClippingListViewModel
    {
        public LocalClippingListViewModel(ILocalClipboardHandler localClipboardHandler)
            : base(localClipboardHandler)
        {
        }
    }
}