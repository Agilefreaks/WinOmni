namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Reactive.Linq;
    using Omnipaste.Helpers;
    using Omnipaste.Models;
    
    public static class ObservableExtensionMethods
    {
        public static IObservable<RepositoryOperation<T>> Created<T>(
            this IObservable<RepositoryOperation<T>> operationObservable)
        {
            return operationObservable.OnMethod(RepositoryMethodEnum.Create);
        }

        public static IObservable<RepositoryOperation<T>> Updated<T>(
            this IObservable<RepositoryOperation<T>> operationObservable)
        {
            return operationObservable.OnMethod(RepositoryMethodEnum.Update);
        }

        public static IObservable<RepositoryOperation<T>> Deleted<T>(
            this IObservable<RepositoryOperation<T>> operationObservable)
        {
            return operationObservable.OnMethod(RepositoryMethodEnum.Delete);
        }

        public static IObservable<RepositoryOperation<T>> ForContact<T>(
            this IObservable<RepositoryOperation<T>> operationObservable,
            ContactInfo contactInfo) where T : IConversationItem
        {
            return operationObservable.Where(o => o.Item.ContactInfo.UniqueId == contactInfo.UniqueId);
        }

        public static IObservable<RepositoryOperation<T>> OnMethod<T>(
            this IObservable<RepositoryOperation<T>> operationObservable,
            RepositoryMethodEnum method)
        {
            return operationObservable.Where(o => o.RepositoryMethod == method);
        }
    }
}