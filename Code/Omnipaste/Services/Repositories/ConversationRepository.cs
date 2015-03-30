namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Joins;
    using System.Reactive.Linq;
    using Omnipaste.Models;
    using OmniUI.Models;

    public abstract class ConversationRepository : SecurePermanentRepository
    {
        protected ConversationRepository(string blobName)
            : base(blobName)
        {
        }

        public virtual IObservable<RepositoryOperation<T>> GetOperationObservable<T, TLocal, TRemote>()
            where T : ConversationBaseModel
            where TLocal : T
            where TRemote : T
        {
            return
                base.GetOperationObservable<TLocal>()
                    .Select(ro => new RepositoryOperation<T>(ro.RepositoryMethod, ro.GetItem<TLocal>()))
                    .Merge(
                        base.GetOperationObservable<TRemote>()
                            .Select(
                                ro =>
                                new RepositoryOperation<T>(ro.RepositoryMethod, ro.GetItem<TRemote>())));
        }

        public IObservable<RepositoryOperation<T>> Delete<T, TLocal, TRemote>(string id)
            where T : ConversationBaseModel
            where TLocal : T
        {
            return base.Delete<TLocal>(id).Select(ro => new RepositoryOperation<T>(ro.RepositoryMethod, ro.Item));
        }

        public IObservable<IEnumerable<T>> GetAll<T, TLocal, TRemote>()
            where T : ConversationBaseModel
            where TLocal : T
            where TRemote : T
        {
            var pattern = base.GetAll<TLocal>().OfType<IEnumerable<T>>().And(base.GetAll<TRemote>());
            return Observable.When(pattern.Then((locals, remotes) => locals.Concat(remotes)));
        }

        public IObservable<IEnumerable<T>> GetForContact<T, TLocal, TRemote>(ContactInfo contactInfo)
            where T : ConversationBaseModel
            where TLocal : T
            where TRemote : T
        {
            Func<T, bool> contactFilter = item => item.ContactInfoUniqueId == contactInfo.UniqueId;
            var pattern = base.GetAll<TLocal>(contactFilter).OfType<IEnumerable<T>>().And(base.GetAll<TRemote>(contactFilter));

            return Observable.When(pattern.Then((locals, remotes) => locals.Concat(remotes)));
        }

        public IObservable<RepositoryOperation<T>> Save<T, TLocal, TRemote>(T item) 
            where T : BaseModel
            where TRemote : BaseModel
            where TLocal : BaseModel
        {
            var remoteItem = item as TRemote;
            return remoteItem != null
                ? base.Save(remoteItem).Select(ro => new RepositoryOperation<T>(ro.RepositoryMethod, ro.GetItem<TRemote>() as T))
                : base.Save(item as TLocal).Select(ro => new RepositoryOperation<T>(ro.RepositoryMethod, ro.GetItem<TLocal>() as T));
        }
    }
}
