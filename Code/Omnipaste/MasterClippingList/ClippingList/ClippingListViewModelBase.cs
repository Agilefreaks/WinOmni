namespace Omnipaste.MasterClippingList.ClippingList
{
    using System;
    using System.Reactive.Linq;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
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

        private readonly IDisposable _itemAddedSubscription;

        private readonly IDisposable _itemRemovedSubscription;

        #endregion

        #region Constructors and Destructors

        protected ClippingListViewModelBase(IClippingRepository clippingRepository)
        {
            _itemAddedSubscription =
                clippingRepository.OperationObservable.Where(o => CanHandle(o.Item))
                    .Saved()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(o => AddItem(o.Item));

            _itemRemovedSubscription =
                clippingRepository.OperationObservable.Where(o => CanHandle(o.Item))
                    .Deleted()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(o => RemoveItem(o.Item));
        }

        #endregion

        #region Public Properties

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        #endregion

        #region Public Methods and Operators

        public abstract bool CanHandle(ClippingModel clipping);

        public void ShowVideoTutorial()
        {
            ExternalProcessHelper.ShowVideoTutorial();
        }

        public override void Dispose()
        {
            _itemAddedSubscription.Dispose();
            _itemRemovedSubscription.Dispose();
            base.Dispose();
        }

        #endregion

        #region Methods

        protected override IClippingViewModel CreateViewModel(ClippingModel clipping)
        {
            IClippingViewModel clippingViewModel = new ClippingViewModel();
            clippingViewModel.Model = clipping;

            return clippingViewModel;
        }

        #endregion
    }
}