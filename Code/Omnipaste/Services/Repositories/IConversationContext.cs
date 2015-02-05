namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Reactive;
    using Omnipaste.Models;

    public interface IConversationContext
    {
        IObservable<IConversationItem> ItemUpdated { get; }

        IObservable<IConversationItem> ItemAdded { get; }

        IObservable<IConversationItem> ItemRemoved { get; }

        IObservable<IConversationItem> Updated { get; }

        IObservable<IEnumerable<IConversationItem>> GetItems();

        IObservable<Unit> Save(IConversationItem item);
    }
}