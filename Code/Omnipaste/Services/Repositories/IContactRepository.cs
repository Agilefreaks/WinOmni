namespace Omnipaste.Services.Repositories
{
    using System;
    using Omnipaste.Models;

    public interface IContactRepository : IRepository<ContactInfo>
    {
        IObservable<ContactInfo> GetByPhoneNumber(string phoneNumber);

        IObservable<ContactInfo> GetByContactIdOrPhoneNumber(int? contactId, string phoneNumber);

        IObservable<ContactInfo> CreateIfNone(IObservable<ContactInfo> observable, Func<ContactInfo, ContactInfo> builder);
    }
}