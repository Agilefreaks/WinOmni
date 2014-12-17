namespace Omnipaste.Services.Commands
{
    using System;
    using System.Reactive.Linq;
    using Contacts.Api.Resources.v1;
    using Contacts.Handlers;
    using Contacts.Models;

    public class SyncContactsCommand
    {
    }

    public class SyncContactsCommandResult
    {
        #region Public Properties

        public ContactList ContactList { get; set; }

        #endregion
    }

    public class SyncContactsCommandProcessor : ICommandProcessor<SyncContactsCommand, SyncContactsCommandResult>
    {
        #region Fields

        private readonly IContacts _contacts;

        private readonly IContactsHandler _contactsHandler;

        #endregion

        #region Constructors and Destructors

        public SyncContactsCommandProcessor(IContacts contacts, IContactsHandler contactsHandler)
        {
            _contacts = contacts;
            _contactsHandler = contactsHandler;
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<SyncContactsCommandResult> Process(SyncContactsCommand command)
        {
            return
                _contacts.Get()
                    .Catch<ContactList, Exception>(
                        exception =>
                        _contacts.Sync()
                            .Select(_ => _contactsHandler)
                            .Switch())
                    .Select(contactList => new SyncContactsCommandResult { ContactList = contactList });
        }

        #endregion
    }
}