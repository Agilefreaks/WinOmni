﻿namespace Events.Handlers
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Events.Api.Resources.v1;
    using Events.Models;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Models;

    public class EventsHandler : IEventsHandler
    {
        #region Fields

        private readonly Subject<Event> _subject;

        private IDisposable _subscription;

        #endregion

        #region Constructors and Destructors

        public EventsHandler(IEvents events)
        {
            _subject = new Subject<Event>();
            Events = events;
        }

        #endregion

        #region Public Properties

        public IEvents Events { private get; set; }

        #endregion

        #region Public Methods and Operators

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(OmniMessage value)
        {
            Events.Last().Subscribe(
                // OnNext
                n => _subject.OnNext(n),
                // onError
                e => {});
        }

        public void Start(IObservable<OmniMessage> omniMessageObservable)
        {
            SubscribeTo(omniMessageObservable);
        }

        public void Stop()
        {
            try
            {
                _subscription.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public IDisposable Subscribe(IObserver<Event> observer)
        {
            return _subject.Subscribe(observer);
        }

        #endregion

        #region Methods

        private void SubscribeTo(IObservable<OmniMessage> observable)
        {
            _subscription = observable.Where(i => i.Provider == OmniMessageTypeEnum.Notification).Subscribe(this);
        }

        #endregion
    }
}