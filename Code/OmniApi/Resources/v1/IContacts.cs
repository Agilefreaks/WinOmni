namespace OmniApi.Resources.v1
{
    using System;
    using OmniApi.Models;

    public interface IContacts
    {
        IObservable<ContactList> Get();

        IObservable<EmptyModel> Sync();
    }
}