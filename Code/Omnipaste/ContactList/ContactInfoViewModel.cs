namespace Omnipaste.ContactList
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
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Properties;
    using Omnipaste.Services;
    using Omnipaste.Services.Providers;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails;
    using Omnipaste.Workspaces;
    using OmniUI.Details;
    using OmniUI.ExtensionMethods;

    public class ContactInfoViewModel : DetailsViewModelBase<ContactInfoPresenter>, IContactInfoViewModel
    {
        public const string SessionSelectionKey = "PeopleWorkspace_SelectedContact";

        private readonly ISessionManager _sessionManager;

        private string _lastActivityInfo;

        private readonly ISubscriptionsManager _subscriptionsManager;

        private bool _hasNotViewedCalls;

        private bool _hasNotViewedMessages;

        private IConversationPresenter _lastActivity;

        public ContactInfoViewModel(ISessionManager sessionManager)
        {
            _subscriptionsManager = new SubscriptionsManager();
            _sessionManager = sessionManager;
            ClickCommand = new Command(ShowDetails);
        }

        [Inject]
        public IContactRepository ContactRepository { get; set; }

        [Inject]
        public IWorkspaceDetailsViewModelFactory DetailsViewModelFactory { get; set; }

        [Inject]
        public IUiRefreshService UiRefreshService { get; set; }

        [Inject]
        public IConversationProvider ConversationProvider { get; set; }

        public Command ClickCommand { get; set; }

        public bool IsSelected
        {
            get
            {
                return Model.BackingModel.UniqueId == _sessionManager[SessionSelectionKey] as string;
            }
        }

        public IConversationPresenter LastActivity
        {
            get
            {
                return _lastActivity;
            }
            set
            {
                if (Equals(value, _lastActivity))
                {
                    return;
                }
                _lastActivity = value;
                NotifyOfPropertyChange();
                LastActivityInfo = GetActivityInfo(_lastActivity);
            }
        }

        public string LastActivityInfo
        {
            get
            {
                return _lastActivityInfo;
            }
            set
            {
                if (value == _lastActivityInfo)
                {
                    return;
                }
                _lastActivityInfo = value;
                NotifyOfPropertyChange(() => LastActivityInfo);
            }
        }

        public DateTime? LastActivityTime
        {
            get
            {
                return Model.BackingModel.LastActivityTime;
            }
        }

        public bool HasNotViewedCalls
        {
            get
            {
                return _hasNotViewedCalls;
            }
            set
            {
                if (value.Equals(_hasNotViewedCalls))
                {
                    return;
                }
                _hasNotViewedCalls = value;
                NotifyOfPropertyChange(() => HasNotViewedCalls);
            }
        }

        public bool HasNotViewedMessages
        {
            get
            {
                return _hasNotViewedMessages;
            }
            set
            {
                if (value.Equals(_hasNotViewedMessages))
                {
                    return;
                }
                _hasNotViewedMessages = value;
                NotifyOfPropertyChange(() => HasNotViewedMessages);
            }
        }

        public void ShowDetails()
        {
            var detailsViewModel = DetailsViewModelFactory.Create(Model);
            _sessionManager[SessionSelectionKey] = Model.BackingModel.UniqueId;

            this.GetParentOfType<IPeopleWorkspace>().DetailsConductor.ActivateItem(detailsViewModel);
        }

        public void OnLoaded()
        {
            RefreshUi();
            UpdateConversationStatus();
            _subscriptionsManager.Add(
                ConversationProvider.ForContact(Model.BackingModel)
                    .Updated.SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(_ => UpdateConversationStatus()));
            
            _subscriptionsManager.Add(
                UiRefreshService.RefreshObservable.SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(_ => RefreshUi()));
            
            _subscriptionsManager.Add(
                _sessionManager.ItemChangedObservable.Where(arg => arg.Key == SessionSelectionKey)
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(arg => NotifyOfPropertyChange(() => IsSelected)));
        }

        public void OnUnloaded()
        {
            _subscriptionsManager.ClearAll();
        }
        
        protected override void HookModel(ContactInfoPresenter model)
        {
            model.PropertyChanged += OnPropertyChanged;
        }

        protected override void UnhookModel(ContactInfoPresenter model)
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

        private void UpdateConversationStatus()
        {
            _subscriptionsManager.Add(
                ConversationProvider.ForContact(Model.BackingModel)
                    .GetItems()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(
                        item =>
                            {
                                if (item is PhoneCallPresenter)
                                {
                                    HasNotViewedCalls = HasNotViewedCalls || item.WasViewed;
                                }
                                else if (item is SmsMessagePresenter)
                                {
                                    HasNotViewedMessages = HasNotViewedMessages || item.WasViewed;
                                }
                                
                                LastActivity = item;
                            }));
        }

        private void RefreshUi()
        {
            NotifyOfPropertyChange(() => LastActivityTime);
            NotifyOfPropertyChange(() => IsSelected);
        }

        private void SaveChanges()
        {
            ContactRepository.Save(Model.BackingModel).RunToCompletion();
        }

        private static string GetActivityInfo(IConversationPresenter item)
        {
            var result = string.Empty;

            if (item is SmsMessagePresenter)
            {
                result = item.Content;
            }
            else if (item is PhoneCallPresenter)
            {
                result = item.Source == SourceType.Local
                             ? Resources.OutgoingCallLabel
                             : Resources.IncommingCallLabel;
            }

            return result;
        }
    }
}