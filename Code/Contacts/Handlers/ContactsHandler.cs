namespace Contacts.Handlers
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using OmniCommon.Models;

    public class ContactsHandler : IContactsHandler
    {
        #region Fields

        private readonly Subject<Unit> _contactListSubject;

        private IDisposable _subscription;

        #endregion

        #region Constructors and Destructors

        public ContactsHandler()
        {
            _contactListSubject = new Subject<Unit>();
        }

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
            _contactListSubject.OnNext(Unit.Default);
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

        public IDisposable Subscribe(IObserver<Unit> observer)
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