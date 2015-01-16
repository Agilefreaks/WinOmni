namespace PhoneCalls.Resources.v1
{
    using System;
    using global::PhoneCalls.Models;
    using OmniApi.Models;
    using OmniCommon.Interfaces;

    public interface IPhoneCalls : IResource<PhoneCall>
    {
        IObservable<PhoneCall> Call(string phoneNumber);

        IObservable<EmptyModel> EndCall(string callId);
    }
}