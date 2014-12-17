namespace Omnipaste.MasterClippingList.ClippingList
{
    using System;
    using Clipboard.Models;
    using Ninject;
    using OmniCommon.Interfaces;
    using Omnipaste.Clipping;
    using Omnipaste.Helpers;
    using OmniUI.List;

    public abstract class ClippingListViewModelBase : ListViewModelBase<Clipping, IClippingViewModel>,
                                                      IClippingListViewModel
    {
        #region Constructors and Destructors

        protected ClippingListViewModelBase(IObservable<Clipping> entityObservable)
            : base(entityObservable)
        {
        }

        #endregion

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        #region Methods

        protected override IClippingViewModel CreateViewModel(Clipping clipping)
        {
            IClippingViewModel clippingViewModel = new ClippingViewModel();
            clippingViewModel.Model = clipping;

            return clippingViewModel;
        }


        public void ShowVideoTutorial()
        {
            ExternalProcessHelper.ShowVideoTutorial();
        }

        #endregion
    }
}