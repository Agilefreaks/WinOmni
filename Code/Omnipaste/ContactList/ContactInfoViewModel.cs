namespace Omnipaste.ContactList
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Framework.Commands;
    using Omnipaste.Models;
    using Omnipaste.Presenters;
    using Omnipaste.Services;
    using Omnipaste.Services.Repositories;
    using Omnipaste.WorkspaceDetails;
    using Omnipaste.Workspaces;
    using OmniUI.Details;
    using OmniUI.ExtensionMethods;

    public class ContactInfoViewModel : DetailsViewModelBase<ContactInfoPresenter>, IContactInfoViewModel
    {
        private string _lastActivityInfo;

        private readonly List<IDisposable> _subscriptions;

        private DateTime? _lastActivityTime;

        private bool _hasNotViewedCalls;

        private bool _hasNotViewedMessages;

        private IDetailsViewModel _detailsViewModel;

        public ContactInfoViewModel()
        {
            ClickCommand = new Command(ShowDetails);
            _subscriptions = new List<IDisposable>();
        }

        [Inject]
        public IContactRepository ContactRepository { get; set; }

        [Inject]
        public IMessageRepository MessageRepository { get; set; }

        [Inject]
        public ICallRepository CallRepository { get; set; }

        [Inject]
        public IWorkspaceDetailsViewModelFactory DetailsViewModelFactory { get; set; }

        [Inject]
        public IUiRefreshService UiRefreshService { get; set; }

        public Command ClickCommand { get; set; }

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
            _subscriptions.Add(GetLastConversationItem().SubscribeAndHandleErrors(UpdateView));
            _subscriptions.Add(GetConversationOperationObservable(RepositoryMethodEnum.Create).SubscribeAndHandleErrors(UpdateView));
            _subscriptions.Add(GetConversationOperationObservable(RepositoryMethodEnum.Update).SubscribeAndHandleErrors(UpdateView));
            _subscriptions.Add(GetConversationOperationObservable(RepositoryMethodEnum.Delete).SubscribeAndHandleErrors(UpdateView));
            _subscriptions.Add(UiRefreshService.RefreshObservable.SubscribeAndHandleErrors(_ => RefreshUi()));
        }

        public void OnUnloaded()
        {
            _subscriptions.ForEach(s => s.Dispose());
            _subscriptions.Clear();
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

        private IObservable<IConversationItem> GetLastConversationItem()
        {
            return
                MessageRepository.GetByContact(Model.ContactInfo)
                    .Select(messages => messages.Where(m => !m.IsDeleted).Cast<IConversationItem>())
                    .Do(messages => HasNotViewedMessages = messages.Any(message => !message.WasViewed))
                    .Merge(CallRepository.GetByContact(Model.ContactInfo).Select(calls => calls.Where(m => !m.IsDeleted)).Do(calls => HasNotViewedCalls = calls.Any(call => !call.WasViewed)))
                    .Buffer(2)
                    .Select(
                        itemLists =>
                        itemLists.SelectMany(i => i.ToList())
                            .OrderByDescending(conversationItem => conversationItem.Time)
                            .FirstOrDefault());
        }

        private IObservable<IConversationItem> GetConversationOperationObservable(RepositoryMethodEnum method)
        {
            var observable1 = MessageRepository.OperationObservable.OnMethod(method);
            var observable2 = CallRepository.OperationObservable.OnMethod(method);
            return
                observable1.ForContact(Model.ContactInfo)
                    .Select(o => o.Item)
                    .Merge(observable2.ForContact(Model.ContactInfo).Select(o => o.Item).Cast<IConversationItem>())
                    .Select(_ => GetLastConversationItem())
                    .Switch();
        }

        private void RefreshUi()
        {
            NotifyOfPropertyChange(() => LastActivityTime);
        }

        private void UpdateView(IConversationItem item)
        {
            if (item == null)
            {
                LastActivityInfo = string.Empty;
                LastActivityTime = null;
            }
            else
            {
                LastActivityInfo = GetActivityInfo(item);
                LastActivityTime = item.Time;
            }
        }

        private void SaveChanges()
        {
            ContactRepository.Save(Model.ContactInfo).RunToCompletion();
        }

        private string GetActivityInfo(IConversationItem item)
        {
            var result = string.Empty;

            if (item is Message)
            {
                result = item.Content;
            }
            else if (item is Call)
            {
                result = item.Source == SourceType.Local
                             ? Properties.Resources.OutgoingCallLabel
                             : Properties.Resources.IncommingCallLabel;
            }

            return result;
        }
    }
}