namespace Omnipaste.ActivityList
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.Activity;
    using Omnipaste.Helpers;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Properties;
    using Omnipaste.Services;
    using Omnipaste.Services.Repositories;
    using OmniUI.List;

    public class ActivityListViewModel : ListViewModelBase<ActivityPresenter, IActivityViewModel>, IActivityListViewModel
    {
        #region Fields

        private readonly IActivityViewModelFactory _activityViewModelFactory;

        private ActivityTypeEnum _allowedActivityTypes;

        private string _filterText;

        private bool _showCalls;

        private bool _showClippings;

        private bool _showMessages;

        private readonly IDisposable _itemRemovedSubscription;

        private readonly IDisposable _itemAddedSubscription;

        #endregion

        #region Constructors and Destructors

        public ActivityListViewModel(
            IClippingRepository clippingRepository,
            IMessageRepository messageRepository,
            ICallRepository callRepository,
            IActivityViewModelFactory activityViewModelFactory,
            IUpdaterService updaterService)
        {
            _activityViewModelFactory = activityViewModelFactory;
            _allowedActivityTypes = ActivityTypeEnum.All;

            _itemAddedSubscription = GetItemAddedObservable(clippingRepository, messageRepository, callRepository, updaterService)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(AddItem);

            _itemRemovedSubscription = GetItemRemovedObservable(clippingRepository, messageRepository, callRepository)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(RemoveItem);
        }

        #endregion

        #region Public Properties

        public override string DisplayName
        {
            get
            {
                return Resources.Activity;
            }
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

        #endregion

        #region Public Methods and Operators

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

        protected override IActivityViewModel CreateViewModel(ActivityPresenter model)
        {
            return _activityViewModelFactory.Create(model);
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

        private IObservable<ActivityPresenter> GetItemAddedObservable(
            IClippingRepository clippingRepository,
            IMessageRepository messageRepository,
            ICallRepository callRepository,
            IUpdaterService updaterService)
        {
            return
                clippingRepository.OperationObservable.Saved().Select(o => new ActivityPresenter(new Activity(o.Item)))
                    .Merge(messageRepository.OperationObservable.Saved().Select(o => new ActivityPresenter(new Activity(o.Item))))
                    .Merge(callRepository.OperationObservable.Saved().Select(o => new ActivityPresenter(new Activity(o.Item))))
                    .Merge(updaterService.UpdateObservable.Select(updateInfo => new ActivityPresenter(new Activity(updateInfo))));
        }

        private IObservable<ActivityPresenter> GetItemRemovedObservable(
            IClippingRepository clippingRepository,
            IMessageRepository messageRepository,
            ICallRepository callRepository)
        {
            return
                clippingRepository.OperationObservable.Deleted()
                    .Select(o => GetActivity(ActivityTypeEnum.Clipping, o.Item.UniqueId))
                    .Merge(messageRepository.OperationObservable.Deleted().Select(o => GetActivity(ActivityTypeEnum.Message, o.Item.UniqueId)))
                    .Merge(callRepository.OperationObservable.Deleted().Select(o => GetActivity(ActivityTypeEnum.Call, o.Item.UniqueId)))
                    .Where(activity => activity != null);
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
                          .All(filterPart => viewModel.Model.ToString()
                                  .RemoveDiacritics()
                                  .IndexOf(filterPart, StringComparison.CurrentCultureIgnoreCase) >= 0);
        }

        #endregion
    }
}