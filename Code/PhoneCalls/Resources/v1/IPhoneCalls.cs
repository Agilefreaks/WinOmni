namespace PhoneCalls.Resources.v1
{
    using System;
    using global::PhoneCalls.Models;
    using OmniApi.Models;

    public interface IPhoneCalls
    {
        IObservable<PhoneCall> Get(string id);

        IObservable<EmptyModel> Call(string phoneNumber);

        IObservable<EmptyModel> EndCall(string callId);
    }
}