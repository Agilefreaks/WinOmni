namespace Omnipaste.Services.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using Contacts.Api.Resources.v1;
    using Contacts.Handlers;
    using Contacts.Models;
    using Ninject;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;

    public class SyncContactsCommand : ICommand<IList<ContactList>>
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

        public IObservable<IList<ContactList>> Execute()
        {
            var deviceInfoList = ConfigurationService.DeviceInfos;
            return
                deviceInfoList.Select(
                    deviceInfo =>
                    Observable.Defer(
                        () =>
                        Contacts.Get(deviceInfo.Identifier)
                            .Catch<ContactList, Exception>(_ => SyncContacts(deviceInfo.Identifier))))
                    .Concat()
                    .Buffer(2);
        }

        #endregion

        #region Methods

        private IObservable<ContactList> SyncContacts(string deviceIdentifier)
        {
            return
                Syncs.Post(new Sync { Identifier = deviceIdentifier, What = SyncWhatEnum.Contacts })
                    .Select(_ => ContactsHandler.Take(1))
                    .Switch()
                    .Select(_ => Contacts.Get(deviceIdentifier))
                    .Switch();
        }

        #endregion
    }
}