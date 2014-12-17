namespace Omnipaste.Services.Commands
{
    using System;

    public interface ICommandProcessor<in TCommand, out TResult>
    {
        IObservable<TResult> Process(TCommand command);
    }
}