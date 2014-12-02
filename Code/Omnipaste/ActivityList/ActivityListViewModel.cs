namespace Omnipaste.ActivityList
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Windows.Data;
    using Castle.Core.Internal;
    using Clipboard.Handlers;
    using Events.Handlers;
    using Omnipaste.Activity;
    using Omnipaste.Activity.Models;
    using Omnipaste.Framework;
    using Omnipaste.Properties;

    public class ActivityListViewModel : ListViewModelBase<Activity, IActivityViewModel>, IActivityListViewModel
    {
        #region Fields

        private readonly IActivityViewModelFactory _activityViewModelFactory;

        private readonly ICollectionView _filteredItems;

        private ActivityTypeEnum _allowedActivityTypes;

        private bool _showCalls;

        private bool _showClippings;

        private bool _showMessages;

        #endregion

        #region Constructors and Destructors

        public ActivityListViewModel(
            IClipboardHandler clipboardHandler,
            IEventsHandler eventsHandler,
            IActivityViewModelFactory activityViewModelFactory)
            : base(GetActivityObservable(clipboardHandler, eventsHandler))
        {
            _activityViewModelFactory = activityViewModelFactory;
            _filteredItems = CollectionViewSource.GetDefaultView(Items);
            _filteredItems.Filter = ShouldShowViewModel;
            Items.CollectionChanged += OnItemsChanged;
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

        #region Public Methods and Operators

        public override void Dispose()
        {
            Items.CollectionChanged -= OnItemsChanged;
            base.Dispose();
        }

        #endregion

        #region Methods

        protected override IActivityViewModel CreateViewModel(Activity entity)
        {
            return _activityViewModelFactory.Create(entity);
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
            return (activityViewModel != null) && _allowedActivityTypes.HasFlag(activityViewModel.Model.Type)
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

        private void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            if (eventArgs.Action == NotifyCollectionChangedAction.Remove
                || eventArgs.Action == NotifyCollectionChangedAction.Reset
                || eventArgs.Action == NotifyCollectionChangedAction.Replace)
            {
                eventArgs.OldItems.Cast<IActivityViewModel>().ForEach(item => item.Dispose());
            }
        }

        #endregion
    }
}