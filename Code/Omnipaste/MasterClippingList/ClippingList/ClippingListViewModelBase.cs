namespace Omnipaste.MasterClippingList.ClippingList
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using Ninject;
    using OmniCommon.Interfaces;
    using Omnipaste.Clipping;
    using Omnipaste.Helpers;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using OmniUI.List;

    public abstract class ClippingListViewModelBase : ListViewModelBase<ClippingModel, IClippingViewModel>,
                                                      IClippingListViewModel
    {
        #region Fields

        private readonly IClippingRepository _clippingRepository;

        #endregion

        #region Public Properties

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        #endregion

        #region Constructors and Destructors

        protected ClippingListViewModelBase(IClippingRepository clippingRepository)
        {
            _clippingRepository = clippingRepository;
        }

        #endregion

        #region Public Methods and Operators

        public abstract bool CanHandle(ClippingModel clipping);

        public void ShowVideoTutorial()
        {
            ExternalProcessHelper.ShowVideoTutorial();
        }

        #endregion

        #region Methods

        protected override IObservable<IEnumerable<ClippingModel>> GetFetchItemsObservable()
        {
            return _clippingRepository.GetAll().Select(items => items.Where(CanHandle));
        }

        protected override IObservable<ClippingModel> GetItemAddedObservable()
        {
            return _clippingRepository.OperationObservable.Created().Select(o => o.Item).Where(CanHandle);
        }

        protected override IObservable<ClippingModel> GetItemRemovedObservable()
        {
            return _clippingRepository.OperationObservable.Deleted().Select(o => o.Item).Where(CanHandle);
        }

        protected override IClippingViewModel CreateViewModel(ClippingModel model)
        {
            IClippingViewModel clippingViewModel = new ClippingViewModel();
            clippingViewModel.Model = model;

            return clippingViewModel;
        }

        #endregion
    }
}