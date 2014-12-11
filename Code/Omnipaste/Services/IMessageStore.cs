namespace Omnipaste.Services
{
    using System;
    using System.Collections.Generic;
    using Ninject;
    using Omnipaste.Models;

    public interface IMessageStore : IStartable
    {
        void AddMessage(Message message);

        IDictionary<string, List<Message>> Messages { get; }

        IObservable<Message> MessageObservable { get; }
    }
}