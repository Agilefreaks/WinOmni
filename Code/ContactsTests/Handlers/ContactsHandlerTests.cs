namespace ContactsTests.Handlers
{
    using System;
    using System.Reactive;
    using System.Reactive.Subjects;
    using System.Threading;
    using Contacts.Handlers;
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

        private MoqMockingKernel _mockingKernel;

        private IContactsHandler _contactsHandler;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void Setup()
        {
            _mockingKernel = new MoqMockingKernel();

            _mockingKernel.Bind<IContactsHandler>().To<ContactsHandler>().InSingletonScope();

            _contactsHandler = _mockingKernel.Get<IContactsHandler>();
        }

        [Test]
        public void WhenAContactsMessageArrives_SubscriberOnNextIsCalled()
        {
            var observer = new Mock<IObserver<Unit>>();
            var omniMessageObservable = new Subject<OmniMessage>();
            _contactsHandler.Start(omniMessageObservable);
            _contactsHandler.Subscribe(observer.Object);
            DispatcherProvider.Instance = new ImmediateDispatcherProvider();
            var autoResetEvent = new AutoResetEvent(false);
            observer.Setup(o => o.OnNext(It.IsAny<Unit>())).Callback(() => autoResetEvent.Set());

            omniMessageObservable.OnNext(new OmniMessage(OmniMessageTypeEnum.Contacts));

            autoResetEvent.WaitOne(1000);
            observer.Verify(o => o.OnNext(It.IsAny<Unit>()), Times.Once);
        }

        [Test]
        public void WhenAClippingArrives_SubscribeOnNextIsNotCalled()
        {
            var observer = new Mock<IObserver<Unit>>();
            var omniMessageObservable = new Subject<OmniMessage>();
            _contactsHandler.Start(omniMessageObservable);
            _contactsHandler.Subscribe(observer.Object);
            DispatcherProvider.Instance = new ImmediateDispatcherProvider();
            var autoResetEvent = new AutoResetEvent(false);
            observer.Setup(o => o.OnNext(It.IsAny<Unit>())).Callback(() => autoResetEvent.Set());

            omniMessageObservable.OnNext(new OmniMessage(OmniMessageTypeEnum.Clipboard));

            autoResetEvent.WaitOne(1000);
            
            observer.Verify(o => o.OnNext(It.IsAny<Unit>()), Times.Never);
        }

        #endregion
    }
}