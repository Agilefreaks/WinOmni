﻿namespace OmniApi.Resources.v1
{
    using System;
    using OmniApi.Models;

    public interface IPhones
    {
        #region Public Methods and Operators

        IObservable<EmptyModel> EndCall();

        IObservable<EmptyModel> SendSms(string phoneNumber, string content);

        #endregion
    }
}