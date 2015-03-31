namespace Omnipaste.Presenters.Factories
{
    using System;
    using System.Reactive.Linq;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Services;
    using Omnipaste.Services.Repositories;

    public class ActivityPresenterFactory : IActivityPresenterFactory
    {
        private readonly IContactRepository _contactRepository;

        public ActivityPresenterFactory(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public IObservable<ActivityPresenter> Create(ClippingEntity clippingEntity)
        {
            var activityPresenter = ActivityPresenter.BeginBuild(clippingEntity)
                .WithType(ActivityTypeEnum.Clipping)
                .WithContent(clippingEntity.Content)
                .WithDevice(clippingEntity.Source)
                .Build();
            return Observable.Return(activityPresenter);
        }

        public IObservable<ActivityPresenter> Create(PhoneCallEntity phoneCallEntity)
        {
            return _contactRepository.Get(phoneCallEntity.ContactInfoUniqueId).Select(
                contactInfo => ActivityPresenter.BeginBuild(phoneCallEntity)
                           .WithType(ActivityTypeEnum.Call)
                           .WithContactInfo(contactInfo)
                           .WithDevice(phoneCallEntity)
                           .Build());
        }

        public IObservable<ActivityPresenter> Create(SmsMessageEntity smsMessageEntity)
        {
            return _contactRepository.Get(smsMessageEntity.ContactInfoUniqueId).Select(
                contactInfo => ActivityPresenter.BeginBuild(smsMessageEntity)
                    .WithContactInfo(contactInfo)
                    .WithType(ActivityTypeEnum.Message)
                    .WithDevice(smsMessageEntity)
                    .WithContent(smsMessageEntity.Content ?? String.Empty)
                    .Build());
        }

        public IObservable<ActivityPresenter> Create(UpdateInfo updateInfo)
        {
            var activityPresenter = ActivityPresenter.BeginBuild(updateInfo)
                .WithType(ActivityTypeEnum.Version)
                .WithContent(updateInfo)
                .Build();
            return Observable.Return(activityPresenter);
        }
    }
}