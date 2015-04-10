namespace Omnipaste.ClippingList.Clipping
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.Framework.Commands;
    using Omnipaste.Framework.ExtensionMethods;
    using Omnipaste.Framework.Models;
    using Omnipaste.Services;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails;
    using Omnipaste.Workspaces.Clippings;
    using OmniUI.Details;
    using OmniUI.Framework;
    using OmniUI.Framework.ExtensionMethods;

    public class ClippingViewModel : DetailsViewModelBase<ClippingModel>, IClippingViewModel
    {
        public const string SessionSelectionKey = "ClippingWorkspace_SelectedClipping";

        private readonly ISessionManager _sessionManager;

        private readonly ISubscriptionsManager _subscriptionsManager;

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

        public bool IsSelected
        {
            get
            {
                return Model.BackingEntity.UniqueId == _sessionManager[SessionSelectionKey] as string;
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

        #region IClippingViewModel Members

        public DateTime Time
        {
            get
            {
                return Model.Time;
            }
        }

        #endregion

        public void ShowDetails()
        {
            var detailsViewModel = DetailsViewModelFactory.Create(Model.BackingEntity);
            _sessionManager[SessionSelectionKey] = Model.BackingEntity.UniqueId;

            this.GetParentOfType<IClippingsWorkspace>().DetailsConductor.ActivateItem(detailsViewModel);
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

        protected override void HookModel(ClippingModel model)
        {
            model.PropertyChanged += OnPropertyChanged;
        }

        protected override void UnhookModel(ClippingModel model)
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
            ClippingRepository.Save(Model.BackingEntity).RunToCompletion();
        }
    }
}