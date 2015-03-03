﻿namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Models;

    public static class RepositoryExtensionMethods
    {
        public static IObservable<IEnumerable<T>> GetByContact<T>(this IRepository<T> repository, ContactInfo contactInfo)
            where T : BaseModel, IConversationItem
        {
            return repository.GetAll(item => item.ContactInfo.UniqueId == contactInfo.UniqueId);
        }
    }
}
