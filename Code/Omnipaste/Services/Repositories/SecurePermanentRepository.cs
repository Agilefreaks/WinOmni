namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reactive.Linq;
    using Akavache;
    using Akavache.Sqlite3;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using OmniUI.Entities;
    using Splat;

    public class SecurePermanentRepository : BaseRepositroy
    {
        public IBlobCache Cache { get; private set; }

        public SecurePermanentRepository(String blobName)
        {
            Cache = ModeDetector.InUnitTestRunner()
                            ? (IBlobCache)new InMemoryBlobCache(SchedulerProvider.Default)
                            : new SQLiteEncryptedBlobCache(
                                  Path.Combine(GetDefaultLocalMachineCacheDirectory(), blobName),
                                  new EncryptionProvider(),
                                  SchedulerProvider.Default);
            DefaultExpireIn = new TimeSpan(1, 0, 0, 0);
        }

        public TimeSpan? DefaultExpireIn { get; set; }

        private string GetDefaultLocalMachineCacheDirectory()
        {
            var defaultLocalMachineCacheDirectory =
                Path.Combine(
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        Constants.PublisherName),
                    Constants.AppName,
                    "Blobs");

            if (!Directory.Exists(defaultLocalMachineCacheDirectory))
            {
                Directory.CreateDirectory(defaultLocalMachineCacheDirectory);
            }

            return defaultLocalMachineCacheDirectory;
        }

        public override IObservable<RepositoryOperation<T>> Save<T>(T item)
        {
            var insertObject = DefaultExpireIn.HasValue
                                   ? Cache.InsertObject(item.UniqueId, item, DefaultExpireIn.Value)
                                   : Cache.InsertObject(item.UniqueId, item);

            return insertObject
                .Select(_ => RepositoryOperation<T>.Empty())
                .Concat(BuildRepositoryOperation(RepositoryMethodEnum.Changed, item))
                .LastAsync();
        }

        public override IObservable<RepositoryOperation<T>> Delete<T>(string id)
        {
            return Observable.When(
                Get<T>(id)
                .And(Cache.Invalidate(id))
                .Then((item, _) => BuildRepositoryOperation(RepositoryMethodEnum.Delete, item))).Switch();
        }

        public override IObservable<T> Get<T>(string id)
        {
            return Cache.GetObject<T>(id);
        }

        public override IObservable<T> Get<T>(Func<T, bool> match)
        {
            return Cache.GetAllObjects<T>().Select(all => all.First(match));
        }

        public override IObservable<IEnumerable<T>> GetAll<T>()
        {
            return Cache.GetAllObjects<T>();
        }

        public override IObservable<IEnumerable<T>> GetAll<T>(Func<T, bool> filter)
        {
            return Cache.GetAllObjects<T>().Select(e => Observable.Return(e.Where(filter))).Switch();
        }

        private IObservable<RepositoryOperation<T>> BuildRepositoryOperation<T>(RepositoryMethodEnum method, T item)
        {
            var repositoryOperation = new RepositoryOperation<T>(method, item);
            Subject.OnNext(repositoryOperation);

            return Observable.Return(repositoryOperation);
        }
    }

    public abstract class SecurePermanentRepository<T> : BaseRepository<T>
        where T : Entity
    {
        private readonly SecurePermanentRepository _underlyingRepository;

        public IBlobCache Cache
        {
            get
            {
                return _underlyingRepository.Cache;
            }
        }

        protected SecurePermanentRepository(String blobName)
        {
            _underlyingRepository = new SecurePermanentRepository(blobName);
        }

        protected SecurePermanentRepository(string blobName, TimeSpan? defaultExpireIn)
            : this(blobName)
        {
            _underlyingRepository.DefaultExpireIn = defaultExpireIn;
        }

        public override IObservable<RepositoryOperation<T>> GetOperationObservable()
        {
            return _underlyingRepository.GetOperationObservable<T>();
        }

        public override IObservable<RepositoryOperation<T>> Save(T item)
        {
            return _underlyingRepository.Save(item);
        }

        public override IObservable<RepositoryOperation<T>> Delete(string id)
        {
            return _underlyingRepository.Delete<T>(id);
        }

        public override IObservable<T> Get(string id)
        {
            return _underlyingRepository.Get<T>(id);
        }

        public override IObservable<T> Get(Func<T, bool> match)
        {
            return _underlyingRepository.Get(match);
        }

        public override IObservable<IEnumerable<T>> GetAll()
        {
            return _underlyingRepository.GetAll<T>();
        }

        public override IObservable<IEnumerable<T>> GetAll(Func<T, bool> filter)
        {
            return _underlyingRepository.GetAll(filter);
        }
    }
}