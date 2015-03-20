namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Reactive.Linq;
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
            ContactInfo contactInfo) where T : ConversationBaseModel
        {
            return operationObservable.Where(o => o.Item.UniqueId == contactInfo.UniqueId);
        }

        public static IObservable<RepositoryOperation<T>> OnMethod<T>(
            this IObservable<RepositoryOperation<T>> operationObservable,
            RepositoryMethodEnum method)
        {
            return operationObservable.Where(o => o.RepositoryMethod == method);
        }
    }
}