namespace Omnipaste.Activity
{
    using System;
    using System.Reactive.Linq;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Framework;
    using Omnipaste.Framework.Commands;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services;
    using Omnipaste.WorkspaceDetails;
    using Omnipaste.Workspaces;
    using OmniUI.ExtensionMethods;

    public class ActivityViewModel : DetailsViewModelWithAutoRefresh<ActivityPresenter>, IActivityViewModel
    {
        #region Fields

        public const string SessionSelectionKey = "ActivityWorkspace_SelectedActivity";

        private readonly ISubscriptionsManager _subscriptionsManager;

        private readonly ISessionManager _sessionManager;

        private ActivityTypeEnum _activityType;

        private ActivityContentInfo _contentInfo;

        #endregion

        #region Constructors and Destructors

        public ActivityViewModel(IUiRefreshService uiRefreshService, ISessionManager sessionManager)
            : base(uiRefreshService)
        {
            _sessionManager = sessionManager;
            _subscriptionsManager = new SubscriptionsManager();
            ClickCommand = new Command(ShowDetails);
        }

        #endregion

        #region Public Properties

        [Inject]
        public IWorkspaceDetailsViewModelFactory DetailsViewModelFactory { get; set; }

        public ICommand ClickCommand { get; set; }

        public bool IsSelected
        {
            get
            {
                return Model.BackingModel.UniqueId == _sessionManager[SessionSelectionKey] as string;
            }
        }

        public override ActivityPresenter Model
        {
            get
            {
                return base.Model;
            }
            set
            {
                base.Model = value;
                NotifyOfPropertyChange(() => ActivityType);
                UpdateContentInfo();
            }
        }

        public ActivityTypeEnum ActivityType
        {
            get
            {
                return Model.Type;
            }
        }

        public ActivityContentInfo ContentInfo
        {
            get
            {
                return _contentInfo;
            }
            set
            {
                if (Equals(value, _contentInfo))
                {
                    return;
                }
                _contentInfo = value;
                NotifyOfPropertyChange();
            }
        }

        public DateTime Time
        {
            get
            {
                return Model.Time;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void ShowDetails()
        {
            var detailsViewModel = DetailsViewModelFactory.Create(Model);
            EventHandler<DeactivationEventArgs> eventHandler = null;
            eventHandler = (sender, eventArgs) =>
                {
                    detailsViewModel.Deactivated -= eventHandler;
                    UpdateContentInfo();
                };
            detailsViewModel.Deactivated += eventHandler;
            _sessionManager[SessionSelectionKey] = Model.BackingModel.UniqueId;

            this.GetParentOfType<IActivityWorkspace>().DetailsConductor.ActivateItem(detailsViewModel);
            UpdateContentInfo();
        }

        protected override void OnActivate()
        {
            _subscriptionsManager.Add(
                _sessionManager.ItemChangedObservable.Where(arg => arg.Key == SessionSelectionKey)
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(arg => UpdateContentInfo()));

            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            _subscriptionsManager.ClearAll();

            base.OnDeactivate(close);
        }

        #endregion

        #region Methods

        private void UpdateContentInfo()
        {
            if (Model == null) return;

            var contentInfo = new ActivityContentInfo { ContentType = Model.Type };
            if (IsSelected)
            {
                contentInfo.ContentState = ContentStateEnum.Viewing;
            }
            else if (Model.WasViewed)
            {
                contentInfo.ContentState = ContentStateEnum.Viewed;
            }
            else
            {
                contentInfo.ContentState = ContentStateEnum.NotViewed;
            }

            ContentInfo = contentInfo;
        }

        #endregion
    }
}