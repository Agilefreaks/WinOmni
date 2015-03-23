namespace Omnipaste.Factories
{
    using System;
    using Contacts.Models;
    using Omnipaste.Models;

    public interface IContactFactory
    {
        IObservable<ContactInfo> Create(ContactDto contactDto);
    }
}