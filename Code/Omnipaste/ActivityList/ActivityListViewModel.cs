namespace Omnipaste.ActivityList
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Activity;
    using Omnipaste.Helpers;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Properties;
    using Omnipaste.Services.Repositories;
    using OmniUI.List;

    public class ActivityListViewModel : ListViewModelBase<ActivityPresenter, IActivityViewModel>,
                                         IActivityListViewModel
    {
        private readonly IActivityViewModelFactory _activityViewModelFactory;

        private readonly IClippingRepository _clippingRepository;

        private readonly IMessageRepository _messageRepository;

        private readonly IPhoneCallRepository _phoneCallRepository;

        private readonly IUpdateInfoRepository _updateInfoRepository;

        private ActivityTypeEnum _allowedActivityTypes;

        private string _filterText;

        private bool _showCalls;

        private bool _showClippings;

        private bool _showMessages;

        public ActivityListViewModel(
            IClippingRepository clippingRepository,
            IMessageRepository messageRepository,
            IPhoneCallRepository phoneCallRepository,
            IUpdateInfoRepository updateInfoRepository,
            IActivityViewModelFactory activityViewModelFactory)
        {
            _clippingRepository = clippingRepository;
            _messageRepository = messageRepository;
            _phoneCallRepository = phoneCallRepository;
            _activityViewModelFactory = activityViewModelFactory;
            _updateInfoRepository = updateInfoRepository;
            _allowedActivityTypes = ActivityTypeEnum.All;

            FilteredItems.SortDescriptions.Add(new SortDescription("Time", ListSortDirection.Descending));
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

        public bool ShowCalls
        {
            get
            {
                return _showCalls;
            }
            set
            {
                if (value.Equals(_showCalls))
                {
                    return;
                }
                _showCalls = value;
                NotifyOfPropertyChange();
                UpdateFilter();
            }
        }

        public bool ShowClippings
        {
            get
            {
                return _showClippings;
            }
            set
            {
                if (value.Equals(_showClippings))
                {
                    return;
                }
                _showClippings = value;
                NotifyOfPropertyChange();
                UpdateFilter();
            }
        }

        public bool ShowMessages
        {
            get
            {
                return _showMessages;
            }
            set
            {
                if (value.Equals(_showMessages))
                {
                    return;
                }
                _showMessages = value;
                NotifyOfPropertyChange();
                UpdateFilter();
            }
        }

        #region IActivityListViewModel Members

        public override string DisplayName
        {
            get
            {
                return Resources.Activity;
            }
        }

        #endregion

        public void ShowVideoTutorial()
        {
            ExternalProcessHelper.ShowVideoTutorial();
        }

        protected override IActivityViewModel ChangeViewModel(ActivityPresenter model)
        {
            return UpdateViewModel(model) ?? _activityViewModelFactory.Create(model);
        }

        protected override bool CanShow(IActivityViewModel viewModel)
        {
            return MatchesActivityType(viewModel) && MatchesTextFilter(viewModel);
        }

        protected void UpdateFilter()
        {
            _allowedActivityTypes = ActivityTypeEnum.None;
            if (ShowClippings)
            {
                _allowedActivityTypes |= ActivityTypeEnum.Clipping;
            }

            if (ShowCalls)
            {
                _allowedActivityTypes |= ActivityTypeEnum.Call;
            }

            if (ShowMessages)
            {
                _allowedActivityTypes |= ActivityTypeEnum.Message;
            }

            if (_allowedActivityTypes == ActivityTypeEnum.None)
            {
                _allowedActivityTypes = ActivityTypeEnum.All;
            }

            RefreshItems();
        }

        protected override IObservable<IEnumerable<ActivityPresenter>> GetFetchItemsObservable()
        {
            return
                _clippingRepository.GetAll()
                    .Select(items => items.Select(item => new ActivityPresenter(item)))
                    .Merge(
                        _messageRepository.GetAll()
                            .Select(
                                items =>
                                items.Where(item => item.Source == SourceType.Remote)
                                    .Select(item => new ActivityPresenter(item))))
                    .Merge(
                        _phoneCallRepository.GetAll()
                            .Select(
                                items =>
                                items.Where(item => item.Source == SourceType.Remote)
                                    .Select(item => new ActivityPresenter(item))))
                    .Merge(
                        _updateInfoRepository.GetAll()
                            .Select(items => items.Select(item => new ActivityPresenter(item))));
        }

        protected override IObservable<ActivityPresenter> GetItemChangedObservable()
        {
            return
                _clippingRepository.OperationObservable.Changed()
                    .Select(o => new ActivityPresenter(o.Item))
                    .Merge(_messageRepository.OperationObservable.Changed().Select(e => e.Item).OfType<RemoteSmsMessage>().Select(o => new ActivityPresenter(o)))
                    .Merge(_phoneCallRepository.OperationObservable.Changed().Select(e => e.Item).OfType<RemotePhoneCall>().Select(o => new ActivityPresenter(o)))
                    .Merge(_updateInfoRepository.OperationObservable.Changed().Select(o => new ActivityPresenter(o.Item)));
        }

        protected override IObservable<ActivityPresenter> GetItemRemovedObservable()
        {
            return
                _clippingRepository.OperationObservable.Deleted()
                    .Select(o => GetActivity(ActivityTypeEnum.Clipping, o.Item.UniqueId))
                    .Merge(
                        _phoneCallRepository.OperationObservable.Deleted()
                            .Select(o => GetActivity(ActivityTypeEnum.Call, o.Item.UniqueId)))
                    .Merge(
                        _messageRepository.OperationObservable.Deleted()
                            .Select(o => GetActivity(ActivityTypeEnum.Message, o.Item.UniqueId)))
                    .Merge(
                        _phoneCallRepository.OperationObservable.Deleted()
                            .Select(o => GetActivity(ActivityTypeEnum.Call, o.Item.UniqueId)))
                    .Merge(
                        _updateInfoRepository.OperationObservable.Deleted()
                            .Select(o => GetActivity(ActivityTypeEnum.Version, o.Item.UniqueId)));
        }

        private ActivityPresenter GetActivity(ActivityTypeEnum type, string id)
        {
            return
                Items.Select(vm => vm.Model)
                    .FirstOrDefault(activity => activity.Type == type && activity.SourceId == id);
        }

        private bool MatchesActivityType(IActivityViewModel viewModel)
        {
            return (viewModel != null) && _allowedActivityTypes.HasFlag(viewModel.Model.Type)
                   && (viewModel.Model.Type != ActivityTypeEnum.None) && (viewModel.Model.Type != ActivityTypeEnum.All);
        }

        private bool MatchesTextFilter(IActivityViewModel viewModel)
        {
            return string.IsNullOrWhiteSpace(FilterText)
                   || FilterText.Split(' ')
                          .Select(filterPart => filterPart.RemoveDiacritics())
                          .All(
                              filterPart =>
                              viewModel.Model.ToString()
                                  .RemoveDiacritics()
                                  .IndexOf(filterPart, StringComparison.CurrentCultureIgnoreCase) >= 0);
        }

        private IActivityViewModel UpdateViewModel(ActivityPresenter presenter)
        {
            var currentModel = GetActivity(presenter.Type, presenter.SourceId);
            var activityViewModel = GetViewModel(currentModel);
            if (activityViewModel != null)
            {
                activityViewModel.Model = presenter;
            }

            return activityViewModel;
        }
    }
}