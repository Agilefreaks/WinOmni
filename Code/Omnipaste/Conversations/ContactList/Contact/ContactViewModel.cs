namespace Omnipaste.Conversations.ContactList.Contact
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.Framework.Commands;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.ExtensionMethods;
    using Omnipaste.Framework.Models;
    using Omnipaste.Framework.Services;
    using Omnipaste.Framework.Services.Providers;
    using Omnipaste.Framework.Services.Repositories;
    using Omnipaste.Properties;
    using Omnipaste.WorkspaceDetails;
    using OmniUI.Details;
    using OmniUI.Framework;
    using OmniUI.Framework.ExtensionMethods;
    using OmniUI.Framework.Services;
    using OmniUI.Workspaces;

    public class ContactViewModel : DetailsViewModelBase<ContactModel>, IContactViewModel
    {
        #region ContactStatusEnum enum

        public enum ContactStatusEnum
        {
            Normal,

            Selected
        }

        #endregion

        public const string SessionSelectionKey = "PeopleWorkspace_SelectedContact";

        private readonly ISessionManager _sessionManager;

        private readonly ISubscriptionsManager _subscriptionsManager;

        private bool _hasNotViewedCalls;

        private bool _hasNotViewedMessages;

        private bool _isSelected;

        private IConversationModel _lastActivity;

        private string _lastActivityInfo;

        public ContactViewModel(ISessionManager sessionManager)
        {
            _subscriptionsManager = new SubscriptionsManager();
            _sessionManager = sessionManager;
            ClickCommand = new Command(ToggleSelection);
        }

        public ContactStatusEnum State
        {
            get
            {
                return _isSelected ? ContactStatusEnum.Selected : ContactStatusEnum.Normal;
            }
        }

        [Inject]
        public IContactRepository ContactRepository { get; set; }

        [Inject]
        public IDetailsViewModelFactory DetailsViewModelFactory { get; set; }

        [Inject]
        public IUiRefreshService UiRefreshService { get; set; }

        [Inject]
        public IConversationProvider ConversationProvider { get; set; }

        public Command ClickCommand { get; set; }

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

        #region IContactViewModel Members

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

        public void ShowDetails()
        {
            var detailsViewModel = DetailsViewModelFactory.Create(Model.BackingEntity);
            _sessionManager[SessionSelectionKey] = Model.BackingEntity.UniqueId;

            this.GetParentOfType<IMasterDetailsWorkspace>().DetailsConductor.ActivateItem(detailsViewModel);
        }

        #endregion

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
                _sessionManager.ItemChangedObservable.Where(arg => arg.Key == SessionSelectionKey && arg.NewValue != null)
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(arg => IsSelected = (string)arg.NewValue == Model.UniqueId));
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
                result = item.Source == SourceType.Local ? Resources.OutgoingCallLabel : Resources.IncommingCallLabel;
            }

            return result;
        }

        private void ToggleSelection()
        {
            IsSelected = !IsSelected;
        }
    }
}