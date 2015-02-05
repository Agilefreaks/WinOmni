namespace Omnipaste.Services.Providers
{
    using System;
    using System.Reactive;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    public interface IConversationProvider
    {
        IConversationContext ForContact(ContactInfo contactInfo);

        IConversationContext All();

        IObservable<Unit> SaveItem(IConversationItem item);
    }
}