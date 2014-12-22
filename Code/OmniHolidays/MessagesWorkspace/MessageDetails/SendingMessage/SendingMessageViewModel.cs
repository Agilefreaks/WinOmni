namespace OmniHolidays.MessagesWorkspace.MessageDetails.SendingMessage
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reactive.Linq;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using OmniHolidays.ExtensionMethods;
    using OmniHolidays.Resources;
    using OmniHolidays.Services;
    using OmniUI.Presenters;

    public class SendingMessageViewModel : MessageStepViewModelBase, ISendingMessageViewModel
    {
        #region Fields

        private IDisposable _messageSubscription;

        private string _sampleMessage;

        private ObservableCollection<SentMessage> _sentMessages;

        #endregion

        #region Constructors and Destructors

        public SendingMessageViewModel()
        {
            SentMessages = new ObservableCollection<SentMessage>();
        }

        #endregion

        #region Public Properties

        public string SampleMessage
        {
            get
            {
                return _sampleMessage;
            }
            set
            {
                if (value == _sampleMessage)
                {
                    return;
                }
                _sampleMessage = value;
                NotifyOfPropertyChange();
            }
        }

        public ObservableCollection<SentMessage> SentMessages
        {
            get
            {
                return _sentMessages;
            }
            set
            {
                if (Equals(value, _sentMessages))
                {
                    return;
                }
                _sentMessages = value;
                NotifyOfPropertyChange(() => SentMessages);
            }
        }

        [Inject]
        public ITemplateProcessingService TemplateProcessingService { get; set; }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();

            DisposeMessageSubscription();
            _messageSubscription =
                MessageContext.Contacts.Cast<IContactInfoPresenter>()
                    .ToList()
                    .Select(
                        (model, index) => TemplateProcessingService.Process(MessageContext.Template, model.ContactInfo))
                    .ToSequentialDelayedObservable(Constants.SendingMessageInterval)
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(sampleMessage => SentMessages.Insert(0, CreateSentMessage(sampleMessage)));
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                DisposeMessageSubscription();
            }
            base.OnDeactivate(close);
        }

        private SentMessage CreateSentMessage(string sampleMessage)
        {
            return new SentMessage { Text = sampleMessage, Category = MessageContext.MessageCategory };
        }

        private void DisposeMessageSubscription()
        {
            if (_messageSubscription == null)
            {
                return;
            }
            _messageSubscription.Dispose();
            _messageSubscription = null;
        }

        #endregion
    }
}