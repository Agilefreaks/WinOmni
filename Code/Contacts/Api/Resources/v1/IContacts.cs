namespace Contacts.Api.Resources.v1
{
    using System;
    using global::Contacts.Models;

    public interface IContacts
    {
        IObservable<ContactList> Get(string identifier);

        IObservable<ContactList> GetAll();
    }
}