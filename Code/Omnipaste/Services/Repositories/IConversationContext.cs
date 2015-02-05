namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Collections.Generic;
    using Omnipaste.Models;

    public interface IConversationContext
    {
        IObservable<IConversationItem> ItemUpdated { get; }

        IObservable<IConversationItem> ItemAdded { get; }

        IObservable<IConversationItem> ItemRemoved { get; }

        IObservable<IConversationItem> Updated { get; }

        IObservable<IEnumerable<IConversationItem>> GetItems();
    }
}