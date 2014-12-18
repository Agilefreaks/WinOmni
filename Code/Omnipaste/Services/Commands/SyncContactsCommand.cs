namespace Omnipaste.Services.Commands
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using Contacts.Api.Resources.v1;
    using Contacts.Handlers;
    using Contacts.Models;
    using Ninject;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Interfaces;
    
    public class SyncContactsCommand : ICommand<ContactList>
    {
        #region Public Properties

        [Inject]
        public IContacts Contacts { get; set; }

        [Inject]
        public ISyncs Syncs { get; set; }

        [Inject]
        public IContactsHandler ContactsHandler { get; set; }

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        #endregion

        #region Public Methods and Operators

        public IObservable<ContactList> Execute()
        {
            return Contacts.GetAll().Catch<ContactList, Exception>(SyncContacts);
        }

        #endregion

        #region Methods

        private IObservable<ContactList> SyncContacts(Exception e)
        {
            return ConfigurationService.DeviceInfos.Select(
                deviceInfo =>
                Syncs.Post(new Sync { Identifier = deviceInfo.Identifier, What = SyncWhatEnum.Contacts }))
                .CombineLatest()
                .Select(_ => ContactsHandler)
                .Switch();
        }

        #endregion
    }
}