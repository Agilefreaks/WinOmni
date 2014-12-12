namespace Omnipaste.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Events.Handlers;
    using Events.Models;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Models;

    public class InMemoryMessageStore : IMessageStore
    {
        #region Fields

        private readonly IEventsHandler _eventsHandler;

        private readonly IDictionary<string, List<Message>> _messages;

        private IDisposable _eventsSubscription;

        private readonly Subject<Message> _messageSubject;

        #endregion

        #region Constructors and Destructors

        public InMemoryMessageStore(IEventsHandler eventsHandler)
        {
            _eventsHandler = eventsHandler;
            _messageSubject = new Subject<Message>();
            _messages = new Dictionary<string, List<Message>>();
        }

        #endregion

        #region Public Properties

        public IDictionary<string, List<Message>> Messages
        {
            get
            {
                return _messages;
            }
        }

        public IObservable<Message> MessageObservable
        {
            get
            {
                return _messageSubject;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Start()
        {
            _eventsSubscription =
                _eventsHandler.Where(@event => @event.Type == EventTypeEnum.IncomingSmsEvent)
                    .Select(@event => new Message(@event))
                    .SubscribeAndHandleErrors(AddMessage);
        }

        public void Stop()
        {
            if (_eventsSubscription == null)
            {
                return;
            }

            _eventsSubscription.Dispose();
            _eventsSubscription = null;
        }

        public void AddMessage(Message message)
        {
            var phone = message.ContactInfo.Phone ?? string.Empty;
            if (!_messages.ContainsKey(phone))
            {
                _messages.Add(phone, new List<Message>());
            }

            _messages[phone].Add(message);
            _messageSubject.OnNext(message);
        }

        #endregion
    }
}