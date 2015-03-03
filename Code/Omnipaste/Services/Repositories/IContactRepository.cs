namespace Omnipaste.Services.Repositories
{
    using System;
    using Omnipaste.Models;

    public interface IContactRepository : IRepository<ContactInfo>
    {
        IObservable<ContactInfo> GetByPhoneNumber(string phoneNumber);

        IObservable<ContactInfo> GetOrCreateByPhoneNumber(string phoneNumber);
    }
}