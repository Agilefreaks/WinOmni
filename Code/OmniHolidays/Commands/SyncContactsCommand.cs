namespace OmniHolidays.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using Contacts.Api.Resources.v1;
    using Contacts.Handlers;
    using Contacts.Models;
    using Ninject;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Interfaces;
    using OmniUI.Framework.Commands;

    public class SyncContactsCommand : ICommand<ContactList>
    {
        #region Public Properties

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        [Inject]
        public IContacts Contacts { get; set; }

        [Inject]
        public IContactsHandler ContactsHandler { get; set; }

        [Inject]
        public ISyncs Syncs { get; set; }

        #endregion

        #region Public Methods and Operators

        public IObservable<ContactList> Execute()
        {
            return
                Contacts.Get(ConfigurationService.DeviceIdentifier)
                    .Catch<ContactList, Exception>(_ => SyncContacts(ConfigurationService.DeviceIdentifier));
        }

        #endregion

        #region Methods

        private IObservable<ContactList> SyncContacts(string deviceIdentifier)
        {
            return
                Syncs.Post(new Sync(deviceIdentifier))
                    .Select(_ => ContactsHandler.Take(1))
                    .Switch()
                    .Select(_ => Contacts.Get(deviceIdentifier))
                    .Switch();
        }

        #endregion
    }
}