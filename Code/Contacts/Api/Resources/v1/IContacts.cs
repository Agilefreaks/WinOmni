namespace Contacts.Api.Resources.v1
{
    using System;
    using global::Contacts.Models;
    using OmniApi.Models;

    public interface IContacts
    {
        IObservable<ContactList> Get();

        IObservable<EmptyModel> Sync();
    }
}