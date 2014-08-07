namespace OmniApi.Resources.v1
{
    using System;
    using OmniApi.Models;
    using Refit;

    public class Phones : Resource<Phones.IPhonesApi>, IPhones
    {
        #region Interfaces

        [ColdObservable]
        public interface IPhonesApi
        {
            #region Public Methods and Operators

            [Post("/phones/end_call")]
            IObservable<EmptyModel> EndCall([Header("Authorization")] string token);

            #endregion
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<EmptyModel> EndCall()
        {
            return Authorize(ResourceApi.EndCall(AccessToken));
        }

        #endregion
    }
}