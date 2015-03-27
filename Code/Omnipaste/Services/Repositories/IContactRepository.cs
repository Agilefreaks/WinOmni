namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Models;

    public interface IContactRepository : IRepository<ContactInfo>
    {
        IObservable<ContactInfo> GetByPhoneNumber(string phoneNumber);

        IObservable<ContactInfo> GetOrCreateByPhoneNumber(string phoneNumber);

        IObservable<ContactInfo> UpdateLastActivityTime(ContactInfo contactInfo, DateTime? lastActivityTime = null);

        IObservable<ContactInfo> GetOrCreateByPhoneNumbers(IList<string> phoneNumbers);

        IObservable<ContactInfo> GetByContactIdOrPhoneNumber(int? contactId, string phoneNumber);

        IObservable<ContactInfo> CreateIfNone(IObservable<ContactInfo> observable, Func<ContactInfo, ContactInfo> builder);
    }
}