﻿namespace Omnipaste.Framework.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using Castle.Core.Internal;
    using Clipboard.Handlers;
    using Contacts.Handlers;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Entities.Factories;
    using Omnipaste.Framework.Services.Repositories;
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
        public IUpdateRepository UpdateRepository { get; set; }

        [Inject]
        public IContactFactory ContactFactory { get; set; }

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
                    .SubscribeAndHandleErrors(clipping => ClippingRepository.Save(new ClippingEntity(clipping))));

            _subscriptions.Add(
                PhoneCallReceivedHandler.Select(PhoneCallFactory.Create<RemotePhoneCallEntity>)
                    .Switch()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors());

            _subscriptions.Add(
                SmsMessageCreatedHandler.Select(SmsMessageFactory.Create<RemoteSmsMessageEntity>)
                    .Switch()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors());

            _subscriptions.Add(
                UpdaterService.UpdateObservable.SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(updateEntity => UpdateRepository.Save(updateEntity)));

            _subscriptions.Add(
                ContactCreatedHandler
                    .Select(ContactFactory.Create)
                    .Switch()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors());

            _subscriptions.Add(
                ContactsUpdatedHandler
                    .SelectMany(cl => cl.Select(ContactFactory.Create))
                    .Merge()
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors());
        }

        public void Stop()
        {
            _subscriptions.ForEach(s => s.Dispose());
            _subscriptions.Clear();
        }
    }
}