namespace Omnipaste.ClippingList
{
    using Clipboard.Handlers;

    
    public class AllClippingListViewModel : ClippingListViewModelBase, IAllClippingListViewModel
    {
        public AllClippingListViewModel(IClipboardHandler clippingsObservable)
            : base(clippingsObservable)
        {
        }
    }
}