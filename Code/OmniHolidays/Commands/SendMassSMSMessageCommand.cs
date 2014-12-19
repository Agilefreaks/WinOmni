namespace OmniHolidays.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using Ninject;
    using OmniHolidays.Services;
    using OmniUI.Framework.Commands;
    using OmniUI.Models;
    using SMS.Resources.v1;

    public class SendMassSMSMessageCommand : ICommand<Unit>
    {
        #region Fields

        private readonly IList<IContactInfo> _contacts;

        private readonly string _template;

        #endregion

        #region Constructors and Destructors

        public SendMassSMSMessageCommand(string template, IList<IContactInfo> contacts)
        {
            _template = template;
            _contacts = contacts;
        }

        #endregion

        #region Public Properties

        public IEnumerable<IContactInfo> Contacts
        {
            get
            {
                return _contacts;
            }
        }

        [Inject]
        public ISMSMessages SMSMessages { get; set; }

        public string Template
        {
            get
            {
                return _template;
            }
        }

        [Inject]
        public ITemplateProcessingService TemplateProcessingService { get; set; }

        #endregion

        #region Public Methods and Operators

        public IObservable<Unit> Execute()
        {
            var phoneNumbers = Contacts.Select(contact => contact.Phone);
            var messages = Contacts.Select(contact => TemplateProcessingService.Process(Template, contact));

            return SMSMessages.Send(messages, phoneNumbers).Select(_ => new Unit());
        }

        #endregion
    }
}