namespace Omnipaste.ClippingList
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Framework;
    using Omnipaste.Framework.Commands;
    using Omnipaste.Presenters;
    using Omnipaste.Services;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails;
    using Omnipaste.Workspaces;
    using OmniUI.Details;
    using OmniUI.ExtensionMethods;

    public class ClippingViewModel : DetailsViewModelBase<ClippingPresenter>, IClippingViewModel
    {
        public const string SessionSelectionKey = "ClippingWorkspace_SelectedClipping";

        private readonly ISubscriptionsManager _subscriptionsManager;

        private readonly ISessionManager _sessionManager;
        
        private bool _hasNotViewedClippings;

        public ClippingViewModel(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
            _subscriptionsManager = new SubscriptionsManager();
            ClickCommand = new Command(ShowDetails);
        }

        [Inject]
        public IWorkspaceDetailsViewModelFactory DetailsViewModelFactory { get; set; }

        [Inject]
        public IClippingRepository ClippingRepository { get; set; }

        public Command ClickCommand { get; set; }

        public DateTime Time
        {
            get
            {
                return Model.Time;
            }
        }

        public bool IsSelected
        {
            get
            {
                return Model.BackingModel.UniqueId == _sessionManager[SessionSelectionKey] as string;
            }
        }

        public bool HasNotViewedClippings
        {
            get
            {
                return _hasNotViewedClippings;
            }
            set
            {
                if (value.Equals(_hasNotViewedClippings))
                {
                    return;
                }
                _hasNotViewedClippings = value;
                NotifyOfPropertyChange(() => HasNotViewedClippings);
            }
        }

        public void ShowDetails()
        {
            var detailsViewModel = DetailsViewModelFactory.Create(Model);
            _sessionManager[SessionSelectionKey] = Model.BackingModel.UniqueId;

            this.GetParentOfType<IClippingWorkspace>().DetailsConductor.ActivateItem(detailsViewModel);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            _subscriptionsManager.Add(
                _sessionManager.ItemChangedObservable.Where(arg => arg.Key == SessionSelectionKey)
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(arg => NotifyOfPropertyChange(() => IsSelected)));
        }

        protected override void OnDeactivate(bool close)
        {
            _subscriptionsManager.ClearAll();
            base.OnDeactivate(close);
        }

        protected override void HookModel(ClippingPresenter model)
        {
            model.PropertyChanged += OnPropertyChanged;
        }

        protected override void UnhookModel(ClippingPresenter model)
        {
            model.PropertyChanged -= OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Model.GetPropertyName(m => m.IsStarred))
            {
                SaveChanges();
            }
        }

        private void SaveChanges()
        {
            ClippingRepository.Save(Model.BackingModel).RunToCompletion();
        }
    }
}
