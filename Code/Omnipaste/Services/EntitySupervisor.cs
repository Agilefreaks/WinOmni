namespace Omnipaste.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using Castle.Core.Internal;
    using Clipboard.Handlers;
    using Events.Handlers;
    using Events.Models;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using PhoneCalls.Handlers;
    using SMS.Handlers;

    public class EntitySupervisor : IEntitySupervisor
    {
        private readonly IList<IDisposable> _subscriptions;

        [Inject]
        public IClipboardHandler ClipboardHandler { get; set; }

        [Inject]
        public IClippingRepository ClippingRepository { get; set; }

        [Inject]
        public IPhoneCallReceivedHandler PhoneCallReceivedHandler { get; set; }

        [Inject]
        public ICallRepository CallRepository { get; set; }

        [Inject]
        public ISmsMessageCreatedHandler SmsMessageCreatedHandler { get; set; }

        [Inject]
        public IMessageRepository MessageRepository { get; set; }

        [Inject]
        public IUpdaterService UpdaterService { get; set; }

        [Inject]
        public IUpdateInfoRepository UpdateInfoRepository { get; set; }

        public EntitySupervisor()
        {
            _subscriptions = new List<IDisposable>();
        }

        public void Start()
        {
            Stop();

            _subscriptions.Add(
                ClipboardHandler.SubscribeOn(SchedulerProvider.Default)
                .ObserveOn(SchedulerProvider.Default)
                .SubscribeAndHandleErrors(clipping => ClippingRepository.Save(new ClippingModel(clipping))));

            _subscriptions.Add(
                PhoneCallReceivedHandler.SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(phoneCall => CallRepository.Save(new Call(phoneCall))));

            _subscriptions.Add(
                SmsMessageCreatedHandler
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(smsMessage => MessageRepository.Save(new Message(smsMessage))));

            _subscriptions.Add(
                UpdaterService.UpdateObservable.SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(updateInfo => UpdateInfoRepository.Save(updateInfo)));
        }

        public void Stop()
        {
            _subscriptions.ForEach(s => s.Dispose());
            _subscriptions.Clear();
        }
    }
}
