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
    using Splat;

    public abstract class SecurePermanentRepository<T> : BaseRepository<T>
        where T : BaseModel
    {
        private readonly IBlobCache _blobCache;

        protected SecurePermanentRepository(String blobName)
        {
            _blobCache = ModeDetector.InUnitTestRunner()
                             ? (IBlobCache)new InMemoryBlobCache(SchedulerProvider.Default)
                             : new SQLiteEncryptedBlobCache(
                                   Path.Combine(GetDefaultLocalMachineCacheDirectory(), blobName),
                                   new EncryptionProvider(),
                                   SchedulerProvider.Default);
        }

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

        public override IObservable<RepositoryOperation<T>> Save(T item)
        {
            return
                _blobCache.InsertObject(item.UniqueId, item, new TimeSpan(1, 0, 0, 0))
                    .Select(_ => RepositoryOperation<T>.Empty())
                    .Concat(Observable.Return(BuildRepositoryOperation(RepositoryMethodEnum.Changed, item)))
                    .TakeLast(1);
        }

        public override IObservable<RepositoryOperation<T>> Delete(string id)
        {
            return _blobCache.Invalidate(id)
                .Select(_ => RepositoryOperation<T>.Empty())
                .Concat(Observable.Return(BuildRepositoryOperation(RepositoryMethodEnum.Delete, null)))
                .TakeLast(1);
        }

        public override IObservable<T> Get(string id)
        {
            return _blobCache.GetObject<T>(id);
        }

        public override IObservable<T> Get(Func<T, bool> match)
        {
            return _blobCache.GetAllObjects<T>().Select(all => all.First(match));
        }

        public override IObservable<IEnumerable<T>> GetAll()
        {
            return _blobCache.GetAllObjects<T>();
        }

        public override IObservable<IEnumerable<T>> GetAll(Func<T, bool> filter)
        {
            throw new NotImplementedException();
        }

        private RepositoryOperation<T> BuildRepositoryOperation(RepositoryMethodEnum method, T item)
        {
            var repositoryOperation = new RepositoryOperation<T>(method, item);
            Subject.OnNext(repositoryOperation);
            return repositoryOperation;
        }
    }
}