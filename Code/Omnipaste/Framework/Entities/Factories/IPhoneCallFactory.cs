namespace Omnipaste.Framework.Entities.Factories
{
    using System;
    using PhoneCalls.Dto;

    public interface IPhoneCallFactory
    {
        IObservable<T> Create<T>(PhoneCallDto phoneCallDto) where T : PhoneCallEntity;
    }
}