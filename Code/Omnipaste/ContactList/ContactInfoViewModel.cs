namespace Omnipaste.ContactList
{
    using System;
    using System.ComponentModel;
    using System.Linq;
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
    using Message = Omnipaste.Models.Message;

    public class ContactInfoViewModel : DetailsViewModelBase<ContactInfoPresenter>, IContactInfoViewModel
    {
        public const string SessionSelectionKey = "PeopleWorkspace_SelectedContact";

        private readonly ISessionManager _sessionManager;

        private string _lastActivityInfo;

        private readonly ISubscriptionsManager _subscriptionsManager;

        private DateTime? _lastActivityTime;

        private bool _hasNotViewedCalls;

        private bool _hasNotViewedMessages;

        private IConversationItem _lastActivity;

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
                return Model.ContactInfo.UniqueId == _sessionManager[SessionSelectionKey] as string;
            }
        }

        public IConversationItem LastActivity
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
                LastActivityTime = _lastActivity == null ? (DateTime?)null : _lastActivity.Time;
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
                return _lastActivityTime;
            }
            set
            {
                if (value.Equals(_lastActivityTime))
                {
                    return;
                }
                _lastActivityTime = value;
                NotifyOfPropertyChange(() => LastActivityTime);
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
            _sessionManager[SessionSelectionKey] = Model.ContactInfo.UniqueId;

            this.GetParentOfType<IPeopleWorkspace>().DetailsConductor.ActivateItem(detailsViewModel);
        }

        public void OnLoaded()
        {
            UpdateConversationStatus();
            _subscriptionsManager.Add(
                ConversationProvider.ForContact(Model.ContactInfo)
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
                    .Subscribe(arg => NotifyOfPropertyChange(() => IsSelected)));
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
                ConversationProvider.ForContact(Model.ContactInfo)
                    .GetItems()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(
                        items =>
                            {
                                var conversationItems = items.Where(item => !item.IsDeleted).ToList();
                                HasNotViewedCalls = conversationItems.OfType<Call>().Any(item => !item.WasViewed);
                                HasNotViewedMessages = conversationItems.OfType<Message>().Any(item => !item.WasViewed);
                                LastActivity = conversationItems.OrderByDescending(item => item.Time).FirstOrDefault();
                            }));
        }

        private void RefreshUi()
        {
            NotifyOfPropertyChange(() => LastActivityTime);
        }

        private void SaveChanges()
        {
            ContactRepository.Save(Model.ContactInfo).RunToCompletion();
        }

        private static string GetActivityInfo(IConversationItem item)
        {
            var result = string.Empty;

            if (item is Message)
            {
                result = item.Content;
            }
            else if (item is Call)
            {
                result = item.Source == SourceType.Local
                             ? Resources.OutgoingCallLabel
                             : Resources.IncommingCallLabel;
            }

            return result;
        }
    }
}