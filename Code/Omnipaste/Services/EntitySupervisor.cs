namespace Omnipaste.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reactive;
    using System.Reactive.Linq;
    using Castle.Core.Internal;
    using Clipboard.Handlers;
    using Contacts.Handlers;
    using Contacts.Models;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.Factories;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;
    using PhoneCalls.Handlers;
    using SMS.Handlers;

    public class EntitySupervisor : IEntitySupervisor
    {
        private readonly IList<IDisposable> _subscriptions;

        public EntitySupervisor()
        {
            _subscriptions = new List<IDisposable>();
        }

        [Inject]
        public ISmsMessageFactory SmsMessageFactory { get; set; }

        [Inject]
        public IClipboardHandler ClipboardHandler { get; set; }

        [Inject]
        public IClippingRepository ClippingRepository { get; set; }

        [Inject]
        public IPhoneCallReceivedHandler PhoneCallReceivedHandler { get; set; }

        [Inject]
        public IPhoneCallFactory PhoneCallFactory { get; set; }

        [Inject]
        public ISmsMessageCreatedHandler SmsMessageCreatedHandler { get; set; }
        

        [Inject]
        public IUpdaterService UpdaterService { get; set; }

        [Inject]
        public IUpdateInfoRepository UpdateInfoRepository { get; set; }

        [Inject]
        public IContactRepository ContactRepository { get; set; }

        [Inject]
        public IContactCreatedHandler ContactCreatedHandler { get; set; }

        [Inject]
        public IContactsUpdatedHandler ContactsUpdatedHandler { get; set; }

        public void Start()
        {
            Stop();

            _subscriptions.Add(
                ClipboardHandler.SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(clipping => ClippingRepository.Save(new ClippingModel(clipping))));

            _subscriptions.Add(
                PhoneCallReceivedHandler.Select(PhoneCallFactory.Create)
                    .Switch()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors());

            _subscriptions.Add(
                SmsMessageCreatedHandler.Select(SmsMessageFactory.Create<RemoteSmsMessage>)
                    .Switch()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors());

            _subscriptions.Add(
                UpdaterService.UpdateObservable.SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(updateInfo => UpdateInfoRepository.Save(updateInfo)));

            _subscriptions.Add(
                ContactCreatedHandler.SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(c => StoreContact(c)));

            _subscriptions.Add(
                ContactsUpdatedHandler
                    .SelectMany(cl => cl)
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(c => StoreContact(c)));
        }

        public void Stop()
        {
            _subscriptions.ForEach(s => s.Dispose());
            _subscriptions.Clear();
        }

        private IObservable<Unit> StoreContact(ContactDto contactDto)
        {
            var contactInfo = new ContactInfo(contactDto);
            return ContactRepository.Save(contactInfo).Select(_ => Unit.Default);
        }
    }
}