namespace OmniUI.List
{
    using System.Windows.Data;
    using Caliburn.Micro;

    public interface IListViewModel<TViewModel> : IScreen, IConductor, IParent<TViewModel>
    {
        ListCollectionView FilteredItems { get; }

        ListViewModelStatusEnum Status { get; set; }

        void ActivateItem(TViewModel item);

        void RefreshItems();
    }
}