namespace Omnipaste.Services.Repositories
{
    using System;
    using Omnipaste.Entities;
    using Omnipaste.Models;

    public interface IContactRepository : IRepository<ContactEntity>
    {
        IObservable<ContactEntity> GetByPhoneNumber(string phoneNumber);

        IObservable<ContactEntity> GetByContactIdOrPhoneNumber(int? contactId, string phoneNumber);

        IObservable<ContactEntity> CreateIfNone(IObservable<ContactEntity> observable, Func<ContactEntity, ContactEntity> builder);
    }
}