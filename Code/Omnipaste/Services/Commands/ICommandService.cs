namespace Omnipaste.Services.Commands
{
    using System;

    public interface ICommandService
    {
        IObservable<TResult> Execute<TResult>(ICommand<TResult> command);
    }
}
