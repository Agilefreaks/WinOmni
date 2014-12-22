namespace OmniHolidays.MessagesWorkspace.MessageDetails.SendingMessage
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reactive.Linq;
    using Ninject;
    using OmniHolidays.ExtensionMethods;
    using OmniCommon.Helpers;
    using OmniHolidays.MessagesWorkspace.ContactList;
    using OmniHolidays.Resources;
    using OmniHolidays.Services;

    public class SendingMessageViewModel : MessageStepViewModelBase, ISendingMessageViewModel
    {
        private IDisposable _messageSubscription;

        private string _sampleMessage;

        private ObservableCollection<string> _sentMessages;

        public SendingMessageViewModel()
        {
            SentMessages = new ObservableCollection<string>();
        }

        [Inject]
        public ITemplateProcessingService TemplateProcessingService { get; set; }

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

        public ObservableCollection<string> SentMessages
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

        protected override void OnActivate()
        {
            base.OnActivate();

            _messageSubscription =
                MessageContext.Contacts.Cast<IContactViewModel>()
                    .ToList()
                    .Select(
                        (viewModel, index) =>
                        TemplateProcessingService.Process(MessageContext.Template, viewModel.Model.ContactInfo))
                    .ToSequentialDelayedObservable(Constants.SendingMessageInterval)
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .Subscribe(sampleMessage => { SentMessages.Insert(0, sampleMessage); }, () => { });

        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _messageSubscription.Dispose();
        }
    }
}