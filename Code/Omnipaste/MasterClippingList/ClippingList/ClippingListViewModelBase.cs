namespace Omnipaste.MasterClippingList.ClippingList
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using Clipboard.Models;
    using Ninject;
    using OmniCommon.Interfaces;
    using Omnipaste.Clipping;
    using Omnipaste.Framework;

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
            try
            {
                Process.Start(new Uri(ConfigurationService.WebBaseUrl + "/#video").ToString());
            }
            catch (Win32Exception)
            {
                // Looks like there is no way for us to act on this
            }
        }

        #endregion
    }
}