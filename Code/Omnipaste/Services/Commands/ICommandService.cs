namespace Omnipaste.Services.Commands
{
    using System;

    public interface ICommandService
    {
        IObservable<TResult> Execute<TParam, TResult>(TParam param = null)
            where TParam : class
            where TResult : class;
    }
}
