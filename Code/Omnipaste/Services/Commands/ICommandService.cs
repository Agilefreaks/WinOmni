namespace Omnipaste.Services.Commands
{
    using System;

    public interface ICommandService
    {
        IObservable<TResult> Execute<TCommand, TResult>(TCommand command);
    }
}
