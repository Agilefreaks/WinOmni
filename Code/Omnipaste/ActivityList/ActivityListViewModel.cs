namespace Omnipaste.ActivityList
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Clipboard.Handlers;
    using Events.Handlers;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Activity;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Helpers;
    using Omnipaste.Models;
    using Omnipaste.Properties;
    using Omnipaste.Services;
    using OmniUI.List;

    public class ActivityListViewModel : ListViewModelBase<Activity, IActivityViewModel>, IActivityListViewModel
    {
        #region Fields

        private readonly IActivityViewModelFactory _activityViewModelFactory;

        private ActivityTypeEnum _allowedActivityTypes;

        private bool _showCalls;

        private bool _showClippings;

        private bool _showMessages;

        private string _filterText;

        #endregion

        #region Constructors and Destructors

        public ActivityListViewModel(
            IClipboardHandler clipboardHandler,
            IEventsHandler eventsHandler,
            IActivityViewModelFactory activityViewModelFactory,
            IUpdaterService updaterService,
            IEventAggregator eventAggregator)
            : base(GetActivityObservable(clipboardHandler, eventsHandler, updaterService))
        {
            _activityViewModelFactory = activityViewModelFactory;
            _allowedActivityTypes = ActivityTypeEnum.All;
            ViewModelFilter = MatchesFilter;
            eventAggregator.Subscribe(this);
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

        #endregion

        #region Public Methods and Operators

        public void ShowVideoTutorial()
        {
            ExternalProcessHelper.ShowVideoTutorial();
        }

        #endregion

        #region Methods

        protected override IActivityViewModel CreateViewModel(Activity entity)
        {
            return _activityViewModelFactory.Create(entity);
        }

        private static IObservable<Activity> GetActivityObservable(
            IClipboardHandler clipboardHandler,
            IEventsHandler eventsHandler,
            IUpdaterService updaterService)
        {
            return
                clipboardHandler.Select(clipping => new Activity(clipping))
                    .Merge(eventsHandler.Select(@event => new Activity(@event)))
                    .Merge(updaterService.UpdateObservable.Select(updateInfo => new Activity(updateInfo)));
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

            OnFilterUpdated();
        }

        protected bool MatchesFilter(IActivityViewModel viewModel)
        {
            return MatchesActivityType(viewModel) && MatchesTextFilter(viewModel);
        }

        private bool MatchesTextFilter(IActivityViewModel viewModel)
        {
            return string.IsNullOrWhiteSpace(FilterText)
                   || viewModel.Model.ToString()
                          .RemoveDiacritics()
                          .IndexOf(FilterText.RemoveDiacritics(), StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private bool MatchesActivityType(IActivityViewModel viewModel)
        {
            return (viewModel != null) && _allowedActivityTypes.HasFlag(viewModel.Model.Type)
                   && (viewModel.Model.Type != ActivityTypeEnum.None)
                   && (viewModel.Model.Type != ActivityTypeEnum.All);
        }

        #endregion

        public void Handle(DeleteClippingMessage message)
        {
            var activityViewModel = Items.SingleOrDefault(viewModel => viewModel.Model.ExtraData.SourceId == message.ClippingId);
            if (activityViewModel != null)
            {
                DeactivateItem(activityViewModel, true);
            }
        }
    }
}