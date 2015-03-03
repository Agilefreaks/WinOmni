namespace Omnipaste.Factories
{
    using System;
    using Omnipaste.Models;
    using PhoneCalls.Models;

    public interface IPhoneCallFactory
    {
        IObservable<PhoneCall> Create(PhoneCallDto phoneCallDto);
    }
}