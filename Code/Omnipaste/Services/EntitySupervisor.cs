namespace Omnipaste.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reactive;
    using System.Reactive.Linq;
    using Castle.Core.Internal;
    using Clipboard.Handlers;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using PhoneCalls.Handlers;
    using PhoneCalls.Models;
    using SMS.Handlers;
    using SMS.Models;

    public class EntitySupervisor : IEntitySupervisor
    {
        private readonly IList<IDisposable> _subscriptions;

        public EntitySupervisor()
        {
            _subscriptions = new List<IDisposable>();
        }

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

        [Inject]
        public IContactRepository ContactRepository { get; set; }

        public void Start()
        {
            Stop();

            _subscriptions.Add(
                ClipboardHandler.SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(clipping => ClippingRepository.Save(new ClippingModel(clipping))));

            _subscriptions.Add(
                PhoneCallReceivedHandler.Select(StorePhoneCall)
                    .Switch()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors());

            _subscriptions.Add(
                SmsMessageCreatedHandler.Select(StoreMessage)
                    .Switch()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors());

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

        private IObservable<Unit> StoreMessage(SmsMessage smsMessage)
        {
            var message = new Message(smsMessage);
            return
                ContactRepository.GetByPhoneNumber(message.ContactInfo.Phone)
                    .Select(
                        contact =>
                            {
                                return contact == null
                                           ? ContactRepository.Save(message.ContactInfo).Select(_ => Unit.Default)
                                           : Observable.Return(Unit.Default, SchedulerProvider.Default);
                            })
                    .Select(_ => MessageRepository.Save(message).Select(__ => Unit.Default))
                    .Switch();
        }

        private IObservable<Unit> StorePhoneCall(PhoneCall phoneCall)
        {
            var call = new Call(phoneCall);
            return
                ContactRepository.GetByPhoneNumber(call.ContactInfo.Phone)
                    .Select(
                        contact =>
                            {
                                return contact == null
                                           ? ContactRepository.Save(call.ContactInfo).Select(_ => Unit.Default)
                                           : Observable.Return(Unit.Default, SchedulerProvider.Default);
                            })
                    .Switch()
                    .Select(_ => CallRepository.Save(call).Select(__ => Unit.Default))
                    .Switch();
        }
    }
}