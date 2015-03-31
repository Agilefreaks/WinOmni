namespace Omnipaste.Framework.Services.Repositories
{
    using System;
    using System.Reactive.Linq;
    using Akavache;

    public static class BlobCacheExtensions
    {
        public static IObservable<T> GetObjectOrDefault<T>(
            this IBlobCache This,
            string key,
            T defaultValue = default(T))
        {
            return This.GetObject<T>(key).OnErrorResumeNext(Observable.Return(defaultValue));
        }
    }
}