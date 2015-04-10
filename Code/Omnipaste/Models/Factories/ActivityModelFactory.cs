namespace Omnipaste.Models.Factories
{
    using System;
    using System.Reactive.Linq;
    using Omnipaste.ActivityList.Activity;
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
            var activityModel = ActivityModel.BeginBuild(clippingEntity)
                .WithType(ActivityTypeEnum.Clipping)
                .WithContent(clippingEntity.Content)
                .WithDevice(clippingEntity.Source)
                .Build();
            return Observable.Return(activityModel);
        }

        public IObservable<ActivityModel> Create(PhoneCallEntity phoneCallEntity)
        {
            return _contactRepository.Get(phoneCallEntity.ContactUniqueId).Select(
                contactEntity => ActivityModel.BeginBuild(phoneCallEntity)
                           .WithType(ActivityTypeEnum.Call)
                           .WithContact(contactEntity )
                           .WithDevice(phoneCallEntity)
                           .Build());
        }

        public IObservable<ActivityModel> Create(SmsMessageEntity smsMessageEntity)
        {
            return _contactRepository.Get(smsMessageEntity.ContactUniqueId).Select(
                contactEntity => ActivityModel.BeginBuild(smsMessageEntity)
                    .WithContact(contactEntity)
                    .WithType(ActivityTypeEnum.Message)
                    .WithDevice(smsMessageEntity)
                    .WithContent(smsMessageEntity.Content ?? String.Empty)
                    .Build());
        }

        public IObservable<ActivityModel> Create(UpdateEntity updateEntity)
        {
            var activityModel = ActivityModel.BeginBuild(updateEntity)
                .WithType(ActivityTypeEnum.Version)
                .WithContent(updateEntity)
                .Build();
            return Observable.Return(activityModel);
        }
    }
}