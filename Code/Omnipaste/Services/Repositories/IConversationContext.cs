namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Reactive;
    using Omnipaste.Presenters;

    public interface IConversationContext
    {
        IObservable<IConversationPresenter> ItemChanged { get; }

        IObservable<IConversationPresenter> ItemRemoved { get; }

        IObservable<IConversationPresenter> Updated { get; }

        IObservable<IConversationPresenter> GetItems();

        IObservable<Unit> SaveItem(IConversationPresenter item);
    }
}