namespace PhoneCalls.Resources.v1
{
    using System;
    using OmniApi.Models;

    public interface IPhoneCalls
    {
        IObservable<EmptyModel> Call(string phoneNumber);
    }
}