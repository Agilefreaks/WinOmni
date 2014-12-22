namespace OmniHolidays.MessagesWorkspace.MessageDetails.SendingMessage
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using Ninject;
    using OmniHolidays.MessagesWorkspace.ContactList;
    using OmniHolidays.Services;

    public class SendingMessageViewModel : MessageStepViewModelBase, ISendingMessageViewModel
    {
        private IDisposable _messageSubscription;

        private string _sampleMessage;

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

        protected override void OnActivate()
        {
            base.OnActivate();

            _messageSubscription =
                MessageContext.Contacts.Cast<IContactViewModel>()
                    .Select(CreateDelayedObservable)
                    .Concat()
                    .Subscribe(sampleMessage => { SampleMessage = sampleMessage; }, () => { });
        }

        private IObservable<string> CreateDelayedObservable(IContactViewModel contactInfo, int index)
        {
            var compiledMessage = TemplateProcessingService.Process(
                MessageContext.Template,
                contactInfo.Model.ContactInfo);
            
            var result = Observable.Return(compiledMessage);
            if (index > 0)
            {
                result = result.Delay(TimeSpan.FromSeconds(5));
            }

            return result;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _messageSubscription.Dispose();
        }
    }
}