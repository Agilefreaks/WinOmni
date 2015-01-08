namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Models;
    using OmniUI.Models;

    public static class RepositoryExtensionMethods
    {
        public static IObservable<IEnumerable<T>> GetByContact<T>(this IRepository<T> repository, IContactInfo contactInfo)
            where T : BaseModel, IConversationItem
        {
            return repository.GetAll(item => item.ContactInfo.Phone == contactInfo.Phone);
        }
    }
}
