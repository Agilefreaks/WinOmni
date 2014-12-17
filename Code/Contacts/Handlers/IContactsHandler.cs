namespace Contacts.Handlers
{
    using System;
    using Contacts.Models;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public interface IContactsHandler : IObservable<ContactList>, IHandler, IObserver<OmniMessage>
    {
    }
}