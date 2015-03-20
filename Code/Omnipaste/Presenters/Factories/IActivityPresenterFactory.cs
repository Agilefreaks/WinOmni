namespace Omnipaste.Presenters.Factories
{
    using System;
    using Omnipaste.Models;
    using Omnipaste.Services;

    public interface IActivityPresenterFactory
    {
        IObservable<ActivityPresenter> Create(PhoneCall phoneCall);

        IObservable<ActivityPresenter> Create(ClippingModel clippingModel);

        IObservable<ActivityPresenter> Create(SmsMessage smsMessage);

        IObservable<ActivityPresenter> Create(UpdateInfo updateInfo);
    }
}