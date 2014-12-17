namespace Contacts.Handlers
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Contacts.Api.Resources.v1;
    using Contacts.Models;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Models;

    public class ContactsHandler : IContactsHandler
    {
        #region Fields

        private readonly Subject<ContactList> _contactListSubject;

        private IDisposable _subscription;

        #endregion

        #region Constructors and Destructors

        public ContactsHandler(IContacts contacts)
        {
            _contactListSubject = new Subject<ContactList>();
            Contacts = contacts;
        }

        #endregion

        #region Public Properties

        public IContacts Contacts { get; set; }

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
            Contacts.Get().RunToCompletion(n => _contactListSubject.OnNext(n));
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

        public IDisposable Subscribe(IObserver<ContactList> observer)
        {
            return _contactListSubject.Subscribe(observer);
        }

        #endregion

        #region Methods

        private void SubscribeTo(IObservable<OmniMessage> observable)
        {
            _subscription = observable.Where(i => i.Provider == OmniMessageTypeEnum.Contacts).Subscribe(this);
        }

        #endregion
    }
}