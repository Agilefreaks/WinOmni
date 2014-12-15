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

    public class InMemoryStore : IMessageStore, ICallStore
    {
        #region Fields

        private readonly Dictionary<string, List<Call>> _calls;

        private readonly Subject<Call> _callsSubject;

        private readonly IEventsHandler _eventsHandler;

        private readonly Subject<Message> _messageSubject;

        private readonly IDictionary<string, List<Message>> _messages;

        private IDisposable _callsSubscription;

        private IDisposable _messagesSubscription;

        #endregion

        #region Constructors and Destructors

        public InMemoryStore(IEventsHandler eventsHandler)
        {
            _eventsHandler = eventsHandler;
            _messageSubject = new Subject<Message>();
            _callsSubject = new Subject<Call>();
            _messages = new Dictionary<string, List<Message>>();
            _calls = new Dictionary<string, List<Call>>();
        }

        #endregion

        #region Public Properties

        public IObservable<Call> CallObservable
        {
            get
            {
                return _callsSubject;
            }
        }

        public IDictionary<string, List<Call>> Calls
        {
            get
            {
                return _calls;
            }
        }

        public IObservable<Message> MessageObservable
        {
            get
            {
                return _messageSubject;
            }
        }

        public IDictionary<string, List<Message>> Messages
        {
            get
            {
                return _messages;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void AddCall(Call call)
        {
            var phone = call.ContactInfo.Phone ?? string.Empty;
            if (!_calls.ContainsKey(phone))
            {
                _calls.Add(phone, new List<Call>());
            }

            _calls[phone].Add(call);
            _callsSubject.OnNext(call);
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

        public void Start()
        {
            _messagesSubscription =
                _eventsHandler.Where(@event => @event.Type == EventTypeEnum.IncomingSmsEvent)
                    .Select(@event => new Message(@event))
                    .SubscribeAndHandleErrors(AddMessage);
            _callsSubscription =
                _eventsHandler.Where(
                    @event => @event.Type == EventTypeEnum.IncomingCallEvent)
                    .Select(@event => new Call(@event))
                    .SubscribeAndHandleErrors(AddCall);
        }

        public void Stop()
        {
            if (_messagesSubscription != null)
            {
                _messagesSubscription.Dispose();
                _messagesSubscription = null;
            }

            if (_callsSubscription != null)
            {
                _callsSubscription.Dispose();
                _callsSubscription = null;
            }
        }

        #endregion

        #region Explicit Interface Methods

        IEnumerable<Call> ICallStore.GetRelatedCalls(ContactInfo contactInfo)
        {
            return GetRelatedResources(Calls, contactInfo);
        }

        IEnumerable<Message> IMessageStore.GetRelatedMessages(ContactInfo contactInfo)
        {
            return GetRelatedResources(Messages, contactInfo);
        }

        #endregion

        #region Methods

        private static IEnumerable<TResource> GetRelatedResources<TResource>(
            IDictionary<string, List<TResource>> store,
            ContactInfo contactInfo)
        {
            return store.ContainsKey(contactInfo.Phone) ? store[contactInfo.Phone] : new List<TResource>();
        }

        #endregion
    }
}