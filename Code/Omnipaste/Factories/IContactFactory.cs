namespace Omnipaste.Factories
{
    using System;
    using Contacts.Models;
    using Omnipaste.Entities;
    using Omnipaste.Models;

    public interface IContactFactory
    {
        IObservable<ContactEntity> Create(ContactDto contactDto);

        IObservable<ContactEntity> Create(ContactDto contactDto, DateTime? lastActivityTime);
    }
}