namespace OmniHolidays.MessagesWorkspace.MessageDetails
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.Helpers;
    using OmniHolidays.Commands;
    using OmniHolidays.ExtensionMethods;
    using OmniHolidays.MessagesWorkspace.ContactList;
    using OmniHolidays.Resources;
    using OmniUI.ExtensionMethods;
    using OmniUI.Presenters;
    using OmniUI.Services;

    public class MessageDetailsHeaderViewModel : Screen, IMessageDetailsHeaderViewModel
    {
        #region Fields

        private IContactSource _contactSource;

        private IDisposable _sendingMessageSubscription;

        private MessageDetailsHeaderState _state;

        private double _progress;

        #endregion

        #region Public Properties

        [Inject]
        public ICommandService CommandService { get; set; }

        public IContactSource ContactsSource
        {
            get
            {
                return _contactSource;
            }
            set
            {
                if (Equals(value, _contactSource))
                {
                    return;
                }
                UpdateContactSourceHooks(_contactSource, value);
                _contactSource = value;
                NotifyOfPropertyChange();
                UpdateDisplayName();
            }
        }

        public MessageDetailsHeaderState State
        {
            get
            {
                return _state;
            }
            private set
            {
                if (value == _state)
                {
                    return;
                }
                _state = value;
                NotifyOfPropertyChange();
            }
        }

        public int TotalContacts
        {
            get
            {
                return ContactsSource.Contacts.Count;
            }
        }

        public double Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                if (value.Equals(_progress))
                {
                    return;
                }
                _progress = value;
                NotifyOfPropertyChange(() => Progress);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void ClearContacts()
        {
            //This is done due to the fact that the selected contacts collection will be modified when deselecting items
            var contacts = new List<IContactInfoPresenter>(ContactsSource.Contacts.Cast<IContactInfoPresenter>());
            foreach (var contact in contacts)
            {
                contact.IsSelected = false;
            }
        }

        public void Reset()
        {
            CleanUpSendingMessage();
            State = MessageDetailsHeaderState.Normal;
        }

        public void StartNewMessage()
        {
            ClearContacts();
            this.GetParentOfType<IMessageDetailsViewModel>().Reset();
        }

        public void SendMessage(string template)
        {
            Progress = 0;
            State = MessageDetailsHeaderState.Sending;

            CleanUpSendingMessage();
            
            var contactInfos =
                ContactsSource.Contacts.Cast<IContactInfoPresenter>()
                    .Select(contactInfoPresenter => contactInfoPresenter.ContactInfo)
                    .ToList();
            
            _sendingMessageSubscription =
                CommandService.Execute(new SendMassSMSMessageCommand(template, contactInfos))
                    .Select(_ => CreateProgressUpdater(contactInfos.Count * Constants.SendingMessageInterval.TotalMilliseconds))
                    .Switch()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .Subscribe(_ => OnMessageSent(), _ => OnMessageFailed());
        }

        private IObservable<Unit> CreateProgressUpdater(double totalDurationMilliseconds)
        {
            const int UpdateIntervalMilliseconds = 42;
            var totalProgressUpdates = (int)totalDurationMilliseconds / UpdateIntervalMilliseconds + 1;
            var progressIncrement = (double)100 / totalProgressUpdates;
            var updateInterval = TimeSpan.FromMilliseconds(UpdateIntervalMilliseconds);
            return Enumerable.Range(0, totalProgressUpdates)
                .ToSequentialDelayedObservable(updateInterval)
                .Do(_ =>
                            {
                                Progress += progressIncrement;
                            })
                .Buffer(totalProgressUpdates)
                .Select(_ => Unit.Default);
        }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            State = MessageDetailsHeaderState.Normal;
            CleanUpSendingMessage();
            UpdateDisplayName();
            UpdateContactSourceHooks(null, ContactsSource);
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            UpdateContactSourceHooks(ContactsSource, null);

            base.OnDeactivate(close);
        }

        private void CleanUpSendingMessage()
        {
            if (_sendingMessageSubscription == null)
            {
                return;
            }
            _sendingMessageSubscription.Dispose();
            _sendingMessageSubscription = null;
        }

        private void OnMessageFailed()
        {
            StartNewMessage();
        }

        private void OnMessageSent()
        {
            CleanUpSendingMessage();
            State = MessageDetailsHeaderState.Sent;
        }

        private void SelectedContactsOnCollectionChanged(
            object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            UpdateDisplayName();
        }

        private void UpdateContactSourceHooks(IContactSource currentSource, IContactSource newSource)
        {
            if (currentSource != null)
            {
                currentSource.Contacts.CollectionChanged -= SelectedContactsOnCollectionChanged;
            }

            if (newSource != null)
            {
                newSource.Contacts.CollectionChanged += SelectedContactsOnCollectionChanged;
            }
        }

        private void UpdateDisplayName()
        {
            DisplayName =
                ContactsSource.Contacts.Cast<IContactInfoPresenter>()
                    .Select(contact => contact.Identifier)
                    .Aggregate(string.Empty, (current, next) => string.Join(" ", current, next));
        }

        #endregion
    }
}