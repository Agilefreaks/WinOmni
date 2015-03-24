namespace Omnipaste.Presenters.Factories
{
    using System;
    using System.Reactive.Linq;
    using Omnipaste.Models;
    using Omnipaste.Services;

    public class ActivityPresenterFactory : IActivityPresenterFactory
    {
        private readonly IPhoneCallPresenterFactory _phoneCallPresenterFactory;

        private readonly ISmsMessagePresenterFactory _smsMessagePresenterFactory;

        public ActivityPresenterFactory(IPhoneCallPresenterFactory phoneCallPresenterFactory, ISmsMessagePresenterFactory smsMessagePresenterFactory)
        {
            _phoneCallPresenterFactory = phoneCallPresenterFactory;
            _smsMessagePresenterFactory = smsMessagePresenterFactory;
        }

        public IObservable<ActivityPresenter> Create(ClippingModel clippingModel)
        {
            var activityPresenter = ActivityPresenter.BeginBuild()
                .WithType(ActivityTypeEnum.Clipping)
                .WithContent(clippingModel.Content)
                .WithDevice(clippingModel.Source)
                .WithBackingModel(new ClippingPresenter(clippingModel))
                .Build();
            return Observable.Return(activityPresenter);
        }

        public IObservable<ActivityPresenter> Create(PhoneCall phoneCall)
        {
            return _phoneCallPresenterFactory.Create(phoneCall).Select(
                pcp => ActivityPresenter.BeginBuild()
                           .WithType(ActivityTypeEnum.Call)
                           .WithDevice(phoneCall)
                           .WithBackingModel(pcp)
                           .Build());
        }

        public IObservable<ActivityPresenter> Create(SmsMessage smsMessage)
        {
            return _smsMessagePresenterFactory.Create(smsMessage).Select(
                smp => ActivityPresenter.BeginBuild()
                    .WithType(ActivityTypeEnum.Message)
                    .WithDevice(smsMessage)
                    .WithContent(smsMessage.Content ?? String.Empty)
                    .WithBackingModel(smp)
                    .Build());
        }

        public IObservable<ActivityPresenter> Create(UpdateInfo updateInfo)
        {
            var activityPresenter = ActivityPresenter.BeginBuild()
                .WithType(ActivityTypeEnum.Version)
                .WithContent(updateInfo)
                .WithBackingModel(new UpdateInfoPresenter(updateInfo))
                .Build();
            return Observable.Return(activityPresenter);
        }
    }
}