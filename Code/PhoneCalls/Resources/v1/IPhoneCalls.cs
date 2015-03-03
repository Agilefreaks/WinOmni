namespace PhoneCalls.Resources.v1
{
    using System;
    using global::PhoneCalls.Models;
    using OmniApi.Models;
    using OmniCommon.Interfaces;

    public interface IPhoneCalls : IResource<PhoneCallDto>
    {
        IObservable<PhoneCallDto> Call(string phoneNumber, int? contactId = null);

        IObservable<EmptyModel> EndCall(string callId);
    }
}