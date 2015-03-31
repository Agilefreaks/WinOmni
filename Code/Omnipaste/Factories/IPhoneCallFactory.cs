namespace Omnipaste.Factories
{
    using System;
    using Omnipaste.Entities;
    using PhoneCalls.Dto;

    public interface IPhoneCallFactory
    {
        IObservable<T> Create<T>(PhoneCallDto phoneCallDto) where T : PhoneCallEntity;
    }
}