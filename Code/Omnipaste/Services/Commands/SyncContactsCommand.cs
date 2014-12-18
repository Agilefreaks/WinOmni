namespace Omnipaste.Services.Commands
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using Contacts.Api.Resources.v1;
    using Contacts.Handlers;
    using Contacts.Models;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Interfaces;

    public class SyncContactsParam
    {
    }

    public class SyncContactsResult
    {
        #region Public Properties

        public ContactList ContactList { get; set; }

        #endregion
    }

    public class SyncContactsCommand : ICommand<SyncContactsParam, SyncContactsResult>
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        private readonly IContacts _contacts;

        private readonly IContactsHandler _contactsHandler;

        private readonly ISyncs _syncs;

        #endregion

        #region Constructors and Destructors

        public SyncContactsCommand(
            IContacts contacts,
            ISyncs syncs,
            IContactsHandler contactsHandler,
            IConfigurationService configurationService)
        {
            _contacts = contacts;
            _syncs = syncs;
            _contactsHandler = contactsHandler;
            _configurationService = configurationService;
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<SyncContactsResult> Execute(SyncContactsParam param = null)
        {
            return _contacts.GetAll()
                    .Catch<ContactList, Exception>(SyncContacts)
                    .Select(contactList => new SyncContactsResult { ContactList = contactList });
        }

        #endregion

        #region Methods

        private IObservable<ContactList> SyncContacts(Exception e)
        {
            return _configurationService.DeviceInfos.Select(
                deviceInfo =>
                _syncs.Post(new Sync { Identifier = deviceInfo.Identifier, What = SyncWhatEnum.Contacts }))
                .CombineLatest()
                .Select(_ => _contactsHandler)
                .Switch();
        }

        #endregion
    }
}