namespace Omnipaste.ClippingList.AllClippingList
{
    using Clipboard.Handlers;
    using Omnipaste.Framework.Attributes;

    [UseView("Omnipaste.ClippingList.ClippingListView", IsFullyQualifiedName = true)]
    public class AllClippingListViewModel : ClippingListViewModelBase, IAllClippingListViewModel
    {
        public AllClippingListViewModel(IClipboardHandler clippingsObservable)
            : base(clippingsObservable)
        {
        }
    }
}