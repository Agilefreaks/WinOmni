namespace Omnipaste.Services
{
    using System;
    using System.Collections.Generic;
    using Ninject;
    using Omnipaste.Models;

    public interface ICallStore : IStartable
    {
        void AddCall(Call call);

        IDictionary<string, List<Call>> Calls { get; }

        IObservable<Call> CallObservable { get; }

        IEnumerable<Call> GetRelatedCalls(ContactInfo contactInfo);
    }
}