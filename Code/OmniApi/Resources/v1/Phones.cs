namespace OmniApi.Resources.v1
{
    using System;
    using OmniApi.Models;
    using Refit;

    public class Phones : Resource<Phones.IPhonesApi>, IPhones
    {

        [ColdObservable]
        public interface IPhonesApi
        {
            [Post("/phones/end_call")]
            IObservable<Phone> EndCall([Header("Authorization")] string token);
        }

        public IObservable<Phone> EndCall()
        {
            return Authorize(ResourceApi.EndCall(AccessToken));
        }
    }

    public interface IPhones
    {
        IObservable<Phone> EndCall();
    }
}