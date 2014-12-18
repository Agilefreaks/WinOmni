namespace Omnipaste.Services.Commands
{
    using System;

    public interface ICommand<out TResult>
    {
        IObservable<TResult> Execute();
    }
}