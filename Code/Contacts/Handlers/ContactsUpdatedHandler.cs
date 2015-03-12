namespace Contacts.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using Contacts.Api.Resources.v1;
    using Contacts.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Handlers;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public class ContactsUpdatedHandler : ResourceHandler<List<ContactDto>>, IContactsUpdatedHandler
    {
        private readonly IConfigurationService _configurationService;

        private readonly IContacts _contactsResource;

        private readonly IUsers _users;

        private IDisposable _firstRunSubscription;

        public ContactsUpdatedHandler(
            IConfigurationService configurationService,
            IContacts contactsResource,
            IUsers users)
        {
            _configurationService = configurationService;
            _contactsResource = contactsResource;
            _users = users;
        }

        public override string HandledMessageType
        {
            get
            {
                return "contacts_updated";
            }
        }

        public override void OnCompleted()
        {
            base.OnCompleted();

            UpdateUserInfo();
        }

        public override void Start(IObservable<OmniMessage> omniMessageObservable)
        {
            base.Start(omniMessageObservable);

            var userInfo = _configurationService.UserInfo;

            _firstRunSubscription = _users.Get()
                .Where(u => u.ContactsUpdatedAt > userInfo.ContactsUpdatedAt)
                .Select(u => new OmniMessage())
                .Subscribe(this);
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_firstRunSubscription != null)
            {
                _firstRunSubscription.Dispose();
            }
        }

        protected override IObservable<List<ContactDto>> CreateResult(OmniMessage value)
        {
            return SyncContacts();
        }

        protected IObservable<List<ContactDto>> SyncContacts()
        {
            var userInfo = _configurationService.UserInfo;
            return _contactsResource.GetUpdates(userInfo.ContactsUpdatedAt);
        }

        private void UpdateUserInfo()
        {
            var userInfo = _configurationService.UserInfo;
            _users.Get().Subscribe(u => _configurationService.UserInfo = userInfo.SetContactsUpdatedAt(u.ContactsUpdatedAt));
        }
    }
}