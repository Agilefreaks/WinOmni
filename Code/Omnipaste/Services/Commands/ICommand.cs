namespace Omnipaste.Services.Commands
{
    using System;

    public interface ICommand<out TResult>
        where TResult : class
    {
        IObservable<TResult> Execute();
    }
}