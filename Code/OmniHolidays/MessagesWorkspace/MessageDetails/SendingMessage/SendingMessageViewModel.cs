namespace OmniHolidays.MessagesWorkspace.MessageDetails.SendingMessage
{
    using System.Linq;
    using Ninject;
    using OmniHolidays.MessagesWorkspace.ContactList;
    using OmniHolidays.Services;
    using OmniUI.Models;

    public class SendingMessageViewModel : MessageStepViewModelBase, ISendingMessageViewModel
    {
        private string _sampleMessage;

        [Inject]
        public ITemplateProcessingService TemplateProcessingService { get; set; }

        protected override void OnActivate()
        {
            base.OnActivate();
            var contactInfo =
                MessageContext.Contacts.Cast<IContactViewModel>()
                    .Select(viewModel => viewModel.Model.ContactInfo)
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