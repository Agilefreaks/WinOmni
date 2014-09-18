namespace Omnipaste.MasterClippingList.ClippingList
{
    using System;
    using Clipboard.Models;
    using Omnipaste.Clipping;
    using Omnipaste.Framework;

    public abstract class ClippingListViewModelBase : ListViewModelBase<Clipping, IClippingViewModel>, IClippingListViewModel
    {
        #region Constructors and Destructors

        protected ClippingListViewModelBase(IObservable<Clipping> entityObservable) : base(entityObservable)
        {
        }

        #endregion

        #region Methods

        protected override IClippingViewModel CreateViewModel(Clipping clipping)
        {
            IClippingViewModel clippingViewModel = new ClippingViewModel();
            clippingViewModel.Model = clipping;

            return clippingViewModel;
        }
        
        #endregion
    }
}