namespace ContactsTests.Handlers
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading;
    using Contacts.Api.Resources.v1;
    using Contacts.Handlers;
    using Contacts.Models;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using OmniCommon.Models;

    [TestFixture]
    public class ContactsHandlerTests
    {
        #region Fields

        private Mock<IContacts> _mockEvents;

        private MoqMockingKernel _mockingKernel;

        private IContactsHandler _contactsHandler;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void Setup()
        {
            _mockingKernel = new MoqMockingKernel();

            _mockEvents = _mockingKernel.GetMock<IContacts>();
            _mockingKernel.Bind<IContactsHandler>().To<ContactsHandler>().InSingletonScope();

            _contactsHandler = _mockingKernel.Get<IContactsHandler>();
        }

        [Test]
        public void WhenAContactsMessageArrives_SubscriberOnNextIsCalled()
        {
            var observer = new Mock<IObserver<ContactList>>();
            var omniMessageObservable = new Subject<OmniMessage>();
            var contactList = new ContactList();
            _mockEvents.Setup(m => m.GetAll()).Returns(Observable.Return(contactList));
            _contactsHandler.Start(omniMessageObservable);
            _contactsHandler.Subscribe(observer.Object);
            DispatcherProvider.Instance = new ImmediateDispatcherProvider();
            var autoResetEvent = new AutoResetEvent(false);
            observer.Setup(o => o.OnNext(contactList)).Callback(() => autoResetEvent.Set());

            omniMessageObservable.OnNext(new OmniMessage(OmniMessageTypeEnum.Contacts));

            autoResetEvent.WaitOne(1000);
            observer.Verify(o => o.OnNext(contactList), Times.Once);
        }

        [Test]
        public void WhenAClippingArrives_SubscribeOnNextIsNotCalled()
        {
            var observer = new Mock<IObserver<ContactList>>();
            var omniMessageObservable = new Subject<OmniMessage>();
            var contactList = new ContactList();
            _mockEvents.Setup(m => m.GetAll()).Returns(Observable.Return(contactList));
            _contactsHandler.Start(omniMessageObservable);
            _contactsHandler.Subscribe(observer.Object);
            DispatcherProvider.Instance = new ImmediateDispatcherProvider();
            var autoResetEvent = new AutoResetEvent(false);
            observer.Setup(o => o.OnNext(contactList)).Callback(() => autoResetEvent.Set());

            omniMessageObservable.OnNext(new OmniMessage(OmniMessageTypeEnum.Clipboard));

            autoResetEvent.WaitOne(1000);
            
            observer.Verify(o => o.OnNext(It.IsAny<ContactList>()), Times.Never);
        }

        #endregion
    }
}