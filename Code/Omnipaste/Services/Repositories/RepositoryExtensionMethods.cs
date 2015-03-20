namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Models;

    public static class RepositoryExtensionMethods
    {
        public static IObservable<IEnumerable<T>> GetByContact<T>(this IConversationRepository repository, ContactInfo contactInfo)
            where T : ConversationBaseModel
        {
            return repository.GetAll<T>(item => item.ContactInfoUniqueId == contactInfo.UniqueId);
        }
    }
}
