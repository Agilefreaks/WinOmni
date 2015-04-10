namespace Omnipaste.Services.Repositories
{
    using System;
    using System.Reactive;
    using Omnipaste.Framework.Models;

    public interface IConversationContext
    {
        IObservable<IConversationModel> ItemChanged { get; }

        IObservable<IConversationModel> ItemRemoved { get; }

        IObservable<IConversationModel> Updated { get; }

        IObservable<IConversationModel> GetItems();

        IObservable<Unit> SaveItem(IConversationModel item);
    }
}