namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Reactive.Linq;
    using Omnipaste.Entities;
    using Omnipaste.Models;

    public static class RepositoryObservableExtensionMethods
    {
        public static IObservable<RepositoryOperation<T>> Changed<T>(
            this IObservable<RepositoryOperation<T>> operationObservable)
        {
            return operationObservable.OnMethod(RepositoryMethodEnum.Changed);
        }

        public static IObservable<RepositoryOperation<T>> Deleted<T>(
            this IObservable<RepositoryOperation<T>> operationObservable)
        {
            return operationObservable.OnMethod(RepositoryMethodEnum.Delete);
        }

        public static IObservable<RepositoryOperation<T>> ForContact<T>(
            this IObservable<RepositoryOperation<T>> operationObservable,
            ContactEntity contactEntity) where T : ConversationEntity
        {
            return operationObservable.Where(o => o.Item.ContactUniqueId == contactEntity.UniqueId);
        }

        public static IObservable<RepositoryOperation<T>> OnMethod<T>(
            this IObservable<RepositoryOperation<T>> operationObservable,
            RepositoryMethodEnum method)
        {
            return operationObservable.Where(o => o.RepositoryMethod == method);
        }
    }
}