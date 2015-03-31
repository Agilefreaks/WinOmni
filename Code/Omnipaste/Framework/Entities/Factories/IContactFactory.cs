namespace Omnipaste.Framework.Entities.Factories
{
    using System;
    using Contacts.Dto;

    public interface IContactFactory
    {
        IObservable<ContactEntity> Create(ContactDto contactDto);

        IObservable<ContactEntity> Create(ContactDto contactDto, DateTime? lastActivityTime);
    }
}