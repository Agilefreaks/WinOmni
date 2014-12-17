namespace Omnipaste.Services.Commands
{
    using System;

    public interface ICommand<in TParam, out TResult>
        where TParam : class 
        where TResult : class
    {
        IObservable<TResult> Execute(TParam param = null);
    }
}