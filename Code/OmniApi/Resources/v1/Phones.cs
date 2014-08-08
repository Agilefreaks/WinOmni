﻿namespace OmniApi.Resources.v1
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

            [Post("/phones/sms")]
            IObservable<EmptyModel> SendSms(string phone_number, string content, [Header("Authorization")] string token);
            
            #endregion
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<EmptyModel> EndCall()
        {
            return Authorize(ResourceApi.EndCall(AccessToken));
        }

        public IObservable<EmptyModel> SendSms(string phoneNumber, string content)
        {
            return Authorize(ResourceApi.SendSms(phoneNumber, content, AccessToken));
        }

        #endregion
    }
}