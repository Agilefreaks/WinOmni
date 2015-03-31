namespace Omnipaste.ClippingList
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;
    using Clipboard.Dto;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Presenters;
    using Omnipaste.Services.Repositories;
    using OmniUI.List;

    public class ClippingListViewModel : ListViewModelBase<ClippingPresenter, IClippingViewModel>,
                                         IClippingListViewModel
    {
        private readonly IClippingRepository _clippingRepository;

        private readonly IClippingViewModelFactory _clippingViewModelFactory;

        private ClippingFilterTypeEnum _clippingTypeFilter;

        private string _filterText;

        private bool _showCloudClippings;

        private bool _showLocalClippings;

        private bool _showStarred;

        public ClippingListViewModel(
            IClippingRepository clippingRepository,
            IClippingViewModelFactory clippingViewModelFactory)
        {
            _clippingRepository = clippingRepository;
            _clippingViewModelFactory = clippingViewModelFactory;

            FilteredItems.SortDescriptions.Add(new SortDescription(PropertyExtensions.GetPropertyName<IClippingViewModel, DateTime>(vm => vm.Time), ListSortDirection.Descending));
        }

        public string FilterText
        {
            get
            {
                return _filterText;
            }
            set
            {
                if (value == _filterText)
                {
                    return;
                }
                _filterText = value;
                NotifyOfPropertyChange();
                UpdateFilter();
            }
        }

        public bool ShowLocalClippings
        {
            get
            {
                return _showLocalClippings;
            }
            set
            {
                if (value.Equals(_showLocalClippings))
                {
                    return;
                }
                _showLocalClippings = value;
                NotifyOfPropertyChange(() => ShowLocalClippings);
                UpdateFilter();
            }
        }

        public bool ShowCloudClippings
        {
            get
            {
                return _showCloudClippings;
            }
            set
            {
                if (value.Equals(_showCloudClippings))
                {
                    return;
                }
                _showCloudClippings = value;
                NotifyOfPropertyChange(() => ShowCloudClippings);
                UpdateFilter();
            }
        }

        public bool ShowStarred
        {
            get
            {
                return _showStarred;
            }
            set
            {
                if (value.Equals(_showStarred))
                {
                    return;
                }
                _showStarred = value;
                NotifyOfPropertyChange(() => ShowStarred);
                UpdateFilter();
            }
        }

        protected override IObservable<ClippingPresenter> GetFetchItemsObservable()
        {
            return
                _clippingRepository.GetAll()
                    .SelectMany(clippings => clippings.Select(clipping => new ClippingPresenter(clipping)));
        }

        protected override IObservable<ClippingPresenter> GetItemChangedObservable()
        {
            return _clippingRepository.GetOperationObservable().Changed().Select(o => new ClippingPresenter(o.Item));
        }

        protected override IObservable<ClippingPresenter> GetItemRemovedObservable()
        {
            return _clippingRepository.GetOperationObservable().Deleted().Select(o => GetClippingPresenter(o.Item.UniqueId));
        }

        protected override IClippingViewModel ChangeViewModel(ClippingPresenter model)
        {
            return UpdateViewModel(model) ?? _clippingViewModelFactory.Create(model);
        }

        private void UpdateFilter()
        {
            _clippingTypeFilter = ClippingFilterTypeEnum.None;

            if (ShowLocalClippings)
            {
                _clippingTypeFilter |= ClippingFilterTypeEnum.Local;
            }

            if (ShowCloudClippings)
            {
                _clippingTypeFilter |= ClippingFilterTypeEnum.Cloud;
            }

            if (_clippingTypeFilter == ClippingFilterTypeEnum.None)
            {
                _clippingTypeFilter = ClippingFilterTypeEnum.All;
            }

            RefreshItems();
        }

        protected override bool CanShow(IClippingViewModel viewModel)
        {
            return MatchesFilterType(viewModel) && MatchesTextFilter(viewModel) && MatchesStarred(viewModel);
        }

        private bool MatchesStarred(IClippingViewModel viewModel)
        {
            return !ShowStarred || viewModel.Model.IsStarred;
        }

        private bool MatchesTextFilter(IClippingViewModel viewModel)
        {
            return string.IsNullOrWhiteSpace(FilterText)
                   || (viewModel.Model.Content ?? string.Empty).IndexOf(
                       FilterText,
                       StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private bool MatchesFilterType(IClippingViewModel viewModel)
        {
            return _clippingTypeFilter == ClippingFilterTypeEnum.None
                   || (viewModel.Model.BackingModel.Source == ClippingDto.ClippingSourceEnum.Local
                       && _clippingTypeFilter.HasFlag(ClippingFilterTypeEnum.Local))
                   || (viewModel.Model.BackingModel.Source == ClippingDto.ClippingSourceEnum.Cloud
                       && _clippingTypeFilter.HasFlag(ClippingFilterTypeEnum.Cloud));
        }

        private ClippingPresenter GetClippingPresenter(string uniqueId)
        {
            return Items.Select(vm => vm.Model).FirstOrDefault(clipping => clipping.UniqueId == uniqueId);
        }

        private IClippingViewModel UpdateViewModel(ClippingPresenter obj)
        {
            var viewModel = Items.FirstOrDefault(vm => vm.Model.UniqueId == obj.UniqueId);
            if (viewModel != null)
            {
                viewModel.Model = obj;
            }

            return viewModel;
        }
    }
}