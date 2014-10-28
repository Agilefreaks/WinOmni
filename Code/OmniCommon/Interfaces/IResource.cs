namespace OmniCommon.Interfaces
{
    using System;

    public interface IResource<out T>
    {
        IObservable<T> Last();
    }
}