namespace OmniCommon.Interfaces
{
    using System;

    public interface IResource<out T>
    {
        IObservable<T> Get(string id);

        IObservable<T> Last();
    }
}