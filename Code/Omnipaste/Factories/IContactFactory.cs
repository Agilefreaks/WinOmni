namespace Omnipaste.Factories
{
    using System;
    using Contacts.Dto;
    using Omnipaste.Entities;

    public interface IContactFactory
    {
        IObservable<ContactEntity> Create(ContactDto contactDto);

        IObservable<ContactEntity> Create(ContactDto contactDto, DateTime? lastActivityTime);
    }
}