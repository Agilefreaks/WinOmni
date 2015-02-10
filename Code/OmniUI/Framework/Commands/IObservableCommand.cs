namespace OmniUI.Framework.Commands
{
    using System;

    public interface IObservableCommand<out TResult>
    {
        IObservable<TResult> Execute();
    }
}