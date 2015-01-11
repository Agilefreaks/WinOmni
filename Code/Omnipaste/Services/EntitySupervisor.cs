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

    public class EntitySupervisor : IEntitySupervisor
    {
        private readonly IList<IDisposable> _subscriptions;

        [Inject]
        public IClipboardHandler ClipboardHandler { get; set; }

        [Inject]
        public IClippingRepository ClippingRepository { get; set; }

        [Inject]
        public IEventsHandler EventsHandler { get; set; }

        [Inject]
        public ICallRepository CallRepository { get; set; }

        [Inject]
        public IMessageRepository MessageRepository { get; set; }

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
                EventsHandler.Where(@event => @event.Type == EventTypeEnum.IncomingCallEvent)
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(@event => CallRepository.Save(new Call(@event))));

            _subscriptions.Add(
                EventsHandler.Where(@event => @event.Type == EventTypeEnum.IncomingSmsEvent)
                    .SubscribeOn(SchedulerProvider.Default)
                    .ObserveOn(SchedulerProvider.Default)
                    .SubscribeAndHandleErrors(@event => MessageRepository.Save(new Message(@event))));
        }

        public void Stop()
        {
            _subscriptions.ForEach(s => s.Dispose());
            _subscriptions.Clear();
        }
    }
}
