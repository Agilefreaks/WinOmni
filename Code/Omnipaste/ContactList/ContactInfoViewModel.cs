namespace Omnipaste.ContactList
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Framework.Commands;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Properties;
    using Omnipaste.Services;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails;
    using Omnipaste.Workspaces;
    using OmniUI.Details;
    using OmniUI.ExtensionMethods;

    public class ContactInfoViewModel : DetailsViewModelBase<ContactInfoPresenter>, IContactInfoViewModel
    {
        private string _lastActivityInfo;

        private readonly CompositeDisposable _subscriptions;

        private DateTime? _lastActivityTime;

        private bool _hasNotViewedCalls;

        private bool _hasNotViewedMessages;

        private IDetailsViewModel _detailsViewModel;

        private IConversationItem _lastActivity;

        public ContactInfoViewModel()
        {
            ClickCommand = new Command(ShowDetails);
            _subscriptions = new CompositeDisposable();
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

        public IDetailsViewModel DetailsViewModel
        {
            get
            {
                return _detailsViewModel;
            }
            set
            {
                if (Equals(value, _detailsViewModel))
                {
                    return;
                }
                _detailsViewModel = value;
                NotifyOfPropertyChange(() => DetailsViewModel);
            }
        }

        public void ShowDetails()
        {
            DetailsViewModel = DetailsViewModel ?? DetailsViewModelFactory.Create(Model);
            this.GetParentOfType<IPeopleWorkspace>().DetailsConductor.ActivateItem(_detailsViewModel);
        }

        public void OnLoaded()
        {
            UpdateConversationStatus();
            _subscriptions.Add(ConversationProvider.ForContact(Model.ContactInfo).Updated.SubscribeAndHandleErrors(_ => UpdateConversationStatus()));
            _subscriptions.Add(UiRefreshService.RefreshObservable.SubscribeAndHandleErrors(_ => RefreshUi()));
        }

        public void OnUnloaded()
        {
            _subscriptions.Dispose();
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
            _subscriptions.Add(ConversationProvider.ForContact(Model.ContactInfo)
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