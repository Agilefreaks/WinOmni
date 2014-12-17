namespace Omnipaste.Services.Commands
{
    using System;
    using System.Reactive.Linq;
    using Contacts.Api.Resources.v1;
    using Contacts.Handlers;
    using Contacts.Models;

    public class SyncContactsParams
    {
    }

    public class SyncContactsResult
    {
        #region Public Properties

        public ContactList ContactList { get; set; }

        #endregion
    }

    public class SyncContactsCommand : ICommand<SyncContactsParams, SyncContactsResult>
    {
        #region Fields

        private readonly IContacts _contacts;

        private readonly IContactsHandler _contactsHandler;

        #endregion

        #region Constructors and Destructors

        public SyncContactsCommand(IContacts contacts, IContactsHandler contactsHandler)
        {
            _contacts = contacts;
            _contactsHandler = contactsHandler;
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<SyncContactsResult> Execute(SyncContactsParams @params = null)
        {
            return
                _contacts.Get()
                    .Catch<ContactList, Exception>(
                        exception =>
                        _contacts.Sync()
                            .Select(_ => _contactsHandler)
                            .Switch())
                    .Select(contactList => new SyncContactsResult { ContactList = contactList });
        }

        #endregion
    }
}