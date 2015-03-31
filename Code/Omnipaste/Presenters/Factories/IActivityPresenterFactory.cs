namespace Omnipaste.Presenters.Factories
{
    using System;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Services;

    public interface IActivityPresenterFactory
    {
        IObservable<ActivityPresenter> Create(PhoneCallEntity phoneCallEntity);

        IObservable<ActivityPresenter> Create(ClippingEntity clippingEntity);

        IObservable<ActivityPresenter> Create(SmsMessageEntity smsMessageEntity);

        IObservable<ActivityPresenter> Create(UpdateInfo updateInfo);
    }
}