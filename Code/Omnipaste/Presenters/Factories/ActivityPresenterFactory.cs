namespace Omnipaste.Presenters.Factories
{
    using System;
    using System.Reactive.Linq;
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

        public IObservable<ActivityPresenter> Create(ClippingModel clippingModel)
        {
            var activityPresenter = ActivityPresenter.BeginBuild(clippingModel)
                .WithType(ActivityTypeEnum.Clipping)
                .WithContent(clippingModel.Content)
                .WithDevice(clippingModel.Source)
                .Build();
            return Observable.Return(activityPresenter);
        }

        public IObservable<ActivityPresenter> Create(PhoneCall phoneCall)
        {
            return _contactRepository.Get(phoneCall.ContactInfoUniqueId).Select(
                contactInfo => ActivityPresenter.BeginBuild(phoneCall)
                           .WithType(ActivityTypeEnum.Call)
                           .WithContactInfo(contactInfo)
                           .WithDevice(phoneCall)
                           .Build());
        }

        public IObservable<ActivityPresenter> Create(SmsMessage smsMessage)
        {
            return _contactRepository.Get(smsMessage.ContactInfoUniqueId).Select(
                contactInfo => ActivityPresenter.BeginBuild(smsMessage)
                    .WithContactInfo(contactInfo)
                    .WithType(ActivityTypeEnum.Message)
                    .WithDevice(smsMessage)
                    .WithContent(smsMessage.Content ?? String.Empty)
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