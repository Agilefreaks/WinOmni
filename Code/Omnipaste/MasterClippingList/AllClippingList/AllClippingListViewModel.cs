namespace Omnipaste.MasterClippingList.AllClippingList
{
    using Clipboard.Handlers;
    using Omnipaste.Framework.Attributes;
    using Omnipaste.MasterClippingList.ClippingList;

    [UseView("Omnipaste.MasterClippingList.ClippingList.ClippingListView", IsFullyQualifiedName = true)]
    public class AllClippingListViewModel : ClippingListViewModelBase, IAllClippingListViewModel
    {
        public AllClippingListViewModel(IClipboardHandler entityObservable)
            : base(entityObservable)
        {
        }
    }
}