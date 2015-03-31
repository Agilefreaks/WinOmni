namespace PhoneCallsTests.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using OmniCommon.Models;
    using PhoneCalls.Dto;
    using PhoneCalls.Handlers;
    using PhoneCalls.Resources.v1;

    [TestFixture]
    public class PhoneCallReceivedHandlerTests
    {
        private PhoneCallReceivedHandler _subject;

        private Mock<IPhoneCalls> _mockPhoneCalls;

        private TestScheduler _testScheduler;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;

            _mockPhoneCalls = new Mock<IPhoneCalls>();
            _subject = new PhoneCallReceivedHandler(_mockPhoneCalls.Object);
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void WhenAClippingMessageArrives_SubscriberOnNextIsCalled()
        {
            var observer = new Mock<IObserver<PhoneCallDto>>();
            var omniMessageObservable = new Subject<OmniMessage>();
            const string Id = "42";
            var phoneCall = new PhoneCallDto { Id = Id };
            _mockPhoneCalls.Setup(c => c.Get("42")).Returns(Observable.Return(phoneCall));
            _subject.Start(omniMessageObservable);
            _subject.Subscribe(observer.Object);
            DispatcherProvider.Instance = new ImmediateDispatcherProvider();
            var autoResetEvent = new AutoResetEvent(false);
            observer.Setup(o => o.OnNext(phoneCall)).Callback(() => autoResetEvent.Set());

            omniMessageObservable.OnNext(new OmniMessage { Type = "phone_call_received", Payload = new Dictionary<string, string> { { "id", Id } } });

            autoResetEvent.WaitOne(1000);
            observer.Verify(o => o.OnNext(phoneCall), Times.Once);
        }

        [Test]
        public void WhenOtherMessageArrive_SubscriberOnNextIsNotCalled()
        {
            var observer = new Mock<IObserver<PhoneCallDto>>();
            var observable = new Subject<OmniMessage>();

            _subject.Subscribe(observer.Object);

            observable.OnNext(new OmniMessage { Type = "other" });

            observer.Verify(o => o.OnNext(It.IsAny<PhoneCallDto>()), Times.Never);
        }
    }
}
