namespace OmniHolidays.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using Contacts.Models;
    using Ninject;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using OmniUI.Framework.Commands;
    using SMS.Resources.v1;

    public class SendMassSMSMessageCommand : ICommand<Unit>
    {
        #region Fields

        private readonly IList<Contact> _contacts;

        private readonly string _template;

        private readonly Dictionary<string, Func<dynamic, string>> _tagReplacementList;

        private UserInfo _userInfo;

        #endregion

        #region Constructors and Destructors

        public SendMassSMSMessageCommand(string template, IList<Contact> contacts)
        {
            _template = template;
            _contacts = contacts;
            _tagReplacementList = new Dictionary<string, Func<dynamic, string>>
                                      {
                                           { "%UserFirstName%", (context => context.User.FirstName) },
                                           { "%UserLastName%", (context => context.User.LastName) },
                                           { "%ContactFirstName%", (context => context.Contact.FirstName) },
                                           { "%ContactLastName%", (context => context.Contact.LastName) },
                                      };
        }

        #endregion

        #region Public Properties

        [Inject]
        public ISMSMessages SMSMessages { get; set; }

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        public UserInfo UserInfo
        {
            get
            {
                return _userInfo ?? (_userInfo = ConfigurationService.UserInfo);
            }
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<Unit> Execute()
        {
            var phoneNumbers = _contacts.Select(contact => contact.PhoneNumber);
            var messages = _contacts.Select(contact => ReplacePlaceHolders(_template, contact));

            return SMSMessages.Send(messages, phoneNumbers).Select(_ => new Unit());
        }

        #endregion

        #region Methods

        private string ReplacePlaceHolders(string template, Contact contact)
        {
            dynamic context = new ExpandoObject();
            context.User = UserInfo;
            context.Contact = contact;
            return _tagReplacementList.Aggregate(
                template,
                (result, currentEntry) => result.Replace(currentEntry.Key, currentEntry.Value(context)));
        }

        #endregion
    }
}