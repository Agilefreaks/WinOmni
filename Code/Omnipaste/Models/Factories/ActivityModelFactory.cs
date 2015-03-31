namespace Omnipaste.Models.Factories
{
    using System;
    using System.Reactive.Linq;
    using Omnipaste.Entities;
    using Omnipaste.Services.Repositories;

    public interface IActivityModelFactory
    {
        IObservable<ActivityModel> Create(PhoneCallEntity phoneCallEntity);

        IObservable<ActivityModel> Create(ClippingEntity clippingEntity);

        IObservable<ActivityModel> Create(SmsMessageEntity smsMessageEntity);

        IObservable<ActivityModel> Create(UpdateEntity updateEntity);
    }

    public class ActivityModelFactory : IActivityModelFactory
    {
        private readonly IContactRepository _contactRepository;

        public ActivityModelFactory(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public IObservable<ActivityModel> Create(ClippingEntity clippingEntity)
        {
            var activityPresenter = ActivityModel.BeginBuild(clippingEntity)
                .WithType(ActivityTypeEnum.Clipping)
                .WithContent(clippingEntity.Content)
                .WithDevice(clippingEntity.Source)
                .Build();
            return Observable.Return(activityPresenter);
        }

        public IObservable<ActivityModel> Create(PhoneCallEntity phoneCallEntity)
        {
            return _contactRepository.Get(phoneCallEntity.ContactInfoUniqueId).Select(
                contactInfo => ActivityModel.BeginBuild(phoneCallEntity)
                           .WithType(ActivityTypeEnum.Call)
                           .WithContact(contactInfo)
                           .WithDevice(phoneCallEntity)
                           .Build());
        }

        public IObservable<ActivityModel> Create(SmsMessageEntity smsMessageEntity)
        {
            return _contactRepository.Get(smsMessageEntity.ContactInfoUniqueId).Select(
                contactInfo => ActivityModel.BeginBuild(smsMessageEntity)
                    .WithContact(contactInfo)
                    .WithType(ActivityTypeEnum.Message)
                    .WithDevice(smsMessageEntity)
                    .WithContent(smsMessageEntity.Content ?? String.Empty)
                    .Build());
        }

        public IObservable<ActivityModel> Create(UpdateEntity updateEntity)
        {
            var activityPresenter = ActivityModel.BeginBuild(updateEntity)
                .WithType(ActivityTypeEnum.Version)
                .WithContent(updateEntity)
                .Build();
            return Observable.Return(activityPresenter);
        }
    }
}