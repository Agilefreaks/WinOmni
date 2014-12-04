namespace Omnipaste.ActivityList
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;
    using System.Windows.Data;
    using Clipboard.Handlers;
    using Events.Handlers;
    using Omnipaste.Activity;
    using Omnipaste.Framework;
    using Omnipaste.Properties;

    public class ActivityListViewModel : ListViewModelBase<Activity, ActivityViewModel>, IActivityListViewModel
    {
        #region Fields

        private readonly ICollectionView _filteredItems;

        private ActivityTypeEnum _allowedActivityTypes;

        private bool _showCalls;

        private bool _showClippings;

        private bool _showMessages;

        #endregion

        #region Constructors and Destructors

        public ActivityListViewModel(IClipboardHandler clipboardHandler, IEventsHandler eventsHandler)
            : base(GetActivityObservable(clipboardHandler, eventsHandler))
        {
            _filteredItems = CollectionViewSource.GetDefaultView(Items);
            _filteredItems.Filter = ShouldShowViewModel;
            UpdateFilter();
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

        public ICollectionView FilteredItems
        {
            get
            {
                return _filteredItems;
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

        #region Methods

        protected override ActivityViewModel CreateViewModel(Activity entity)
        {
            return new ActivityViewModel { Model = entity };
        }

        private static IObservable<Activity> GetActivityObservable(
            IClipboardHandler clipboardHandler,
            IEventsHandler eventsHandler)
        {
            return
                clipboardHandler.Select(clipping => new Activity(clipping))
                    .Merge(eventsHandler.Select(@event => new Activity(@event)));
        }

        private bool ShouldShowViewModel(object viewModel)
        {
            var activityViewModel = viewModel as IActivityViewModel;
            return (activityViewModel != null)
                && _allowedActivityTypes.HasFlag(activityViewModel.Model.Type)
                && (activityViewModel.Model.Type != ActivityTypeEnum.None)
                && (activityViewModel.Model.Type != ActivityTypeEnum.All);
        }

        private void UpdateFilter()
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

            _filteredItems.Refresh();
        }

        #endregion
    }
}