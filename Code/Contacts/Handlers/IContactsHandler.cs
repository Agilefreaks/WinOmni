namespace Contacts.Handlers
{
    using System;
    using System.Reactive;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public interface IContactsHandler : IObservable<Unit>, IHandler, IObserver<OmniMessage>
    {
    }
}