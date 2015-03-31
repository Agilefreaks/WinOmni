namespace Omnipaste.ContactList.ContactInfo
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.Entities;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Framework.Commands;
    using Omnipaste.Models;
    using Omnipaste.Properties;
    using Omnipaste.Services;
    using Omnipaste.Services.Providers;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails;
    using OmniUI.Details;
    using OmniUI.ExtensionMethods;
    using OmniUI.Framework;
    using OmniUI.Workspace;

    public class ContactInfoViewModel : DetailsViewModelBase<ContactModel>, IContactInfoViewModel
    {
        public const string SessionSelectionKey = "PeopleWorkspace_SelectedContact";

        private readonly ISessionManager _sessionManager;

        private string _lastActivityInfo;

        private readonly ISubscriptionsManager _subscriptionsManager;

        private bool _hasNotViewedCalls;

        private bool _hasNotViewedMessages;

        private IConversationModel _lastActivity;

        private bool _isSelected;

        public ContactInfoStatusEnum State
        {
            get
            {
                return _isSelected ? ContactInfoStatusEnum.Selected : ContactInfoStatusEnum.Normal;
            }
        }

        public ContactInfoViewModel(ISessionManager sessionManager)
        {
            _subscriptionsManager = new SubscriptionsManager();
            _sessionManager = sessionManager;
            ClickCommand = new Command(ToggleSelection);
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
                return _isSelected;
            }
            set
            {
                if (value.Equals(_isSelected))
                {
                    return;
                }
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
                NotifyOfPropertyChange(() => State);
            }
        }

        public IConversationModel LastActivity
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
                return Model.BackingEntity.LastActivityTime;
            }
        }

        public string Identifier
        {
            get
            {
                return Model.Identifier;
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
            var detailsViewModel = DetailsViewModelFactory.Create(Model.BackingEntity);
            _sessionManager[SessionSelectionKey] = Model.BackingEntity.UniqueId;

            this.GetParentOfType<IMasterDetailsWorkspace>().DetailsConductor.ActivateItem(detailsViewModel);
        }

        public void OnLoaded()
        {
            RefreshUi();
            UpdateConversationStatus();
            _subscriptionsManager.Add(
                ConversationProvider.ForContact(Model.BackingEntity)
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
        
        protected override void HookModel(ContactModel model)
        {
            model.PropertyChanged += OnPropertyChanged;
        }

        protected override void UnhookModel(ContactModel model)
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
            HasNotViewedMessages = false;
            HasNotViewedCalls = false;
            _subscriptionsManager.Add(
                ConversationProvider.ForContact(Model.BackingEntity)
                    .GetItems()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(
                        item =>
                            {
                                if (item is PhoneCallModel)
                                {
                                    HasNotViewedCalls = HasNotViewedCalls || !item.WasViewed;
                                }
                                else if (item is SmsMessageModel)
                                {
                                    HasNotViewedMessages = HasNotViewedMessages || !item.WasViewed;
                                }

                                if (LastActivity == null || LastActivity.Time < item.Time)
                                {
                                    LastActivity = item;
                                }
                            }));
        }

        private void RefreshUi()
        {
            NotifyOfPropertyChange(() => LastActivityTime);
            NotifyOfPropertyChange(() => IsSelected);
        }

        private void SaveChanges()
        {
            ContactRepository.Save(Model.BackingEntity).RunToCompletion();
        }

        private static string GetActivityInfo(IConversationModel item)
        {
            var result = string.Empty;

            if (item is SmsMessageModel)
            {
                result = item.Content;
            }
            else if (item is PhoneCallModel)
            {
                result = item.Source == SourceType.Local
                             ? Resources.OutgoingCallLabel
                             : Resources.IncommingCallLabel;
            }

            return result;
        }

        private void ToggleSelection()
        {
            IsSelected = !IsSelected;
        }
    }
}