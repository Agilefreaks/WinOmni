namespace OmniHolidays.MessagesWorkspace.MessageDetails.SendingMessage
{
    using System.Linq;
    using Ninject;
    using OmniHolidays.Services;
    using OmniUI.Models;
    using OmniUI.Presenters;

    public class SendingMessageViewModel : MessageStepViewModelBase, ISendingMessageViewModel
    {
        private string _sampleMessage;

        [Inject]
        public ITemplateProcessingService TemplateProcessingService { get; set; }

        protected override void OnActivate()
        {
            base.OnActivate();
            var contactInfo =
                MessageContext.Contacts.Cast<IContactInfoPresenter>()
                    .Select(contactInfoPresenter => contactInfoPresenter.ContactInfo)
                    .DefaultIfEmpty(new ContactInfo())
                    .First();
            SampleMessage = TemplateProcessingService.Process(MessageContext.Template, contactInfo);
        }

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
    }
}