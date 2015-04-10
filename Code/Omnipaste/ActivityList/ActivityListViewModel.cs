namespace Omnipaste.ActivityList
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.ActivityList.Activity;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Helpers;
    using Omnipaste.Framework.Models;
    using Omnipaste.Framework.Models.Factories;
    using Omnipaste.Properties;
    using Omnipaste.Services.Repositories;
    using OmniUI.List;

    public class ActivityListViewModel : ListViewModelBase<ActivityModel, IActivityViewModel>,
                                         IActivityListViewModel
    {
        private readonly IActivityViewModelFactory _activityViewModelFactory;

        private readonly IClippingRepository _clippingRepository;

        private readonly IPhoneCallRepository _phoneCallRepository;

        private readonly ISmsMessageRepository _smsMessageRepository;

        private readonly IUpdateRepository _updateRepository;

        private readonly IActivityModelFactory _activityModelFactory;

        private ActivityTypeEnum _allowedActivityTypes;

        private string _filterText;

        private bool _showCalls;

        private bool _showClippings;

        private bool _showMessages;

        public ActivityListViewModel(
            IClippingRepository clippingRepository,
            ISmsMessageRepository smsMessageRepository,
            IPhoneCallRepository phoneCallRepository,
            IUpdateRepository updateRepository,
            IActivityModelFactory activityModelFactory,
            IActivityViewModelFactory activityViewModelFactory)
        {
            _clippingRepository = clippingRepository;
            _smsMessageRepository = smsMessageRepository;
            _phoneCallRepository = phoneCallRepository;
            _activityViewModelFactory = activityViewModelFactory;
            _updateRepository = updateRepository;
            _activityModelFactory = activityModelFactory;
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

        protected override IActivityViewModel ChangeViewModel(ActivityModel model)
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

        protected override IObservable<ActivityModel> GetFetchItemsObservable()
        {
            return
                _clippingRepository.GetAll()
                    .SelectMany(items => items.Select(item => _activityModelFactory.Create(item))).Merge()
                    .Merge(
                        _smsMessageRepository.GetAll()
                            .SelectMany(
                                items => items.OfType<RemoteSmsMessageEntity>().Select(item => _activityModelFactory.Create(item))).Merge())
                    .Merge(
                        _phoneCallRepository.GetAll()
                            .SelectMany(
                                items => items.OfType<RemotePhoneCallEntity>().Select(item => _activityModelFactory.Create(item))).Merge())
                    .Merge(
                        _updateRepository.GetAll()
                            .SelectMany(items => items.Select(item => _activityModelFactory.Create(item))).Merge());
        }

        protected override IObservable<ActivityModel> GetItemChangedObservable()
        {
            return
                _clippingRepository.GetOperationObservable()
                    .Changed()
                    .Select(ro => _activityModelFactory.Create(ro.Item)).Merge()
                    .Merge(
                        _smsMessageRepository.GetOperationObservable<RemoteSmsMessageEntity>()
                            .Changed()
                            .Select(ro => _activityModelFactory.Create(ro.Item)).Merge())
                    .Merge(
                        _phoneCallRepository.GetOperationObservable<RemotePhoneCallEntity>()
                            .Changed()
                            .Select(ro => _activityModelFactory.Create(ro.Item)).Merge())
                    .Merge(
                        _updateRepository.GetOperationObservable()
                            .Changed()
                            .Select(ro => _activityModelFactory.Create(ro.Item)).Merge());
        }

        protected override IObservable<ActivityModel> GetItemRemovedObservable()
        {
            return
                _clippingRepository.GetOperationObservable()
                    .Deleted()
                    .Select(o => GetActivityModel(ActivityTypeEnum.Clipping, o.Item.UniqueId))
                    .Merge(
                        _smsMessageRepository.GetOperationObservable()
                            .Deleted()
                            .Select(o => GetActivityModel(ActivityTypeEnum.Message, o.Item.UniqueId)))
                    .Merge(
                        _phoneCallRepository.GetOperationObservable<RemotePhoneCallEntity>()
                            .Deleted()
                            .Select(o => GetActivityModel(ActivityTypeEnum.Call, o.Item.UniqueId)))
                    .Merge(
                        _updateRepository.GetOperationObservable()
                            .Deleted()
                            .Select(o => GetActivityModel(ActivityTypeEnum.Version, o.Item.UniqueId)));
        }

        private ActivityModel GetActivityModel(ActivityTypeEnum type, string id)
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

        private IActivityViewModel UpdateViewModel(ActivityModel model)
        {
            var currentModel = GetActivityModel(model.Type, model.SourceId);
            var activityViewModel = GetViewModel(currentModel);
            if (activityViewModel != null)
            {
                activityViewModel.Model = model;
            }

            return activityViewModel;
        }
    }
}