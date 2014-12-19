namespace OmniUI.Framework.Commands
{
    using System;

    public interface ICommand<out TResult>
    {
        IObservable<TResult> Execute();
    }
}