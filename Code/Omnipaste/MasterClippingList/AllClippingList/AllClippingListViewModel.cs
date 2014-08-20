namespace Omnipaste.MasterClippingList.AllClippingList
{
    using Clipboard.Handlers;
    using Omnipaste.Framework.Attributes;

    [UseView("Omnipaste.MasterClippingList.ClippingListView", IsFullyQualifiedName = true)]
    public class AllClippingListViewModel : ClippingListViewModelBase, IAllClippingListViewModel
    {
        public AllClippingListViewModel(IClipboardHandler entityObservable)
            : base(entityObservable)
        {
        }
    }
}