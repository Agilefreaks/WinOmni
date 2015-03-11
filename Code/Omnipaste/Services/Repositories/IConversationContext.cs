namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Reactive;
    using Omnipaste.Models;

    public interface IConversationContext
    {
        IObservable<IConversationItem> ItemChanged { get; }

        IObservable<IConversationItem> ItemRemoved { get; }

        IObservable<IConversationItem> Updated { get; }

        IObservable<IEnumerable<IConversationItem>> GetItems();

        IObservable<Unit> SaveItem(IConversationItem item);
    }
}