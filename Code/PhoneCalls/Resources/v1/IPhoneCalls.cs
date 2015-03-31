namespace PhoneCalls.Resources.v1
{
    using System;
    using global::PhoneCalls.Dto;
    using OmniApi.Dto;
    using OmniCommon.Interfaces;

    public interface IPhoneCalls : IResource<PhoneCallDto>
    {
        IObservable<PhoneCallDto> Call(string phoneNumber, int? contactId = null);

        IObservable<EmptyDto> EndCall(string callId);
    }
}