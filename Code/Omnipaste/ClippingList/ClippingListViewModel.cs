namespace Omnipaste.ClippingList
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Repositories;
    using OmniUI.List;

    public class ClippingListViewModel : ListViewModelBase<ClippingPresenter, IClippingViewModel>, IClippingListViewModel
    {
        private readonly IClippingRepository _clippingRepository;

        private readonly IClippingViewModelFactory _clippingViewModelFactory;

        public ClippingListViewModel(IClippingRepository clippingRepository, IClippingViewModelFactory clippingViewModelFactory)
        {
            _clippingRepository = clippingRepository;
            _clippingViewModelFactory = clippingViewModelFactory;

            FilteredItems.SortDescriptions.Add(new SortDescription("Time", ListSortDirection.Descending));
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Subscriptions.Add(
                GetItemUpdatedObservable()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(UpdateViewModel));
        }

        protected override IObservable<IEnumerable<ClippingPresenter>> GetFetchItemsObservable()
        {
            return
                _clippingRepository.GetAll()
                    .Select(clippings => clippings.Select(clipping => new ClippingPresenter(clipping)));
        }

        protected override IObservable<ClippingPresenter> GetItemAddedObservable()
        {
            return _clippingRepository.OperationObservable.Created().Select(o => new ClippingPresenter(o.Item));
        }

        private IObservable<ClippingPresenter> GetItemUpdatedObservable()
        {
            return _clippingRepository.OperationObservable.Updated().Select(o => new ClippingPresenter(o.Item));
        }

        protected override IObservable<ClippingPresenter> GetItemRemovedObservable()
        {
            return _clippingRepository.OperationObservable.Deleted().Select(o => GetClippingViewModel(o.Item.UniqueId));
        }

        protected override IClippingViewModel CreateViewModel(ClippingPresenter model)
        {
            return _clippingViewModelFactory.Create(model);
        }

        private ClippingPresenter GetClippingViewModel(string uniqueId)
        {
            return Items.Select(vm => vm.Model).FirstOrDefault(clipping => clipping.UniqueId == uniqueId);
        }

        private void UpdateViewModel(ClippingPresenter obj)
        {
            var viewModel = Items.FirstOrDefault(vm => vm.Model.UniqueId == obj.UniqueId);
            if (viewModel != null)
            {
                viewModel.Model = obj;
            }
        }
    }
}
