namespace OmnipasteTests.Services
{
    using System;
    using System.Reactive;
    using Clipboard.Handlers;
    using Clipboard.Models;
    using Events.Models;
    using Events.Handlers;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using Omnipaste.Services;
    using Omnipaste.Services.Repositories;

    [TestFixture]
    public class EntitySupervisorTests
    {
        private EntitySupervisor _subject;

        private TestScheduler _testScheduler;

        private Mock<IClipboardHandler> _mockClipboardHandler;

        private Mock<IClippingRepository> _mockClippingRepository;

        private Mock<IEventsHandler> _mockEventsHandler;

        private Mock<ICallRepository> _mockCallRepository;

        private Mock<IMessageRepository> _mockMessageRepository;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            
            _mockClipboardHandler = new Mock<IClipboardHandler> { DefaultValue = DefaultValue.Mock };
            _mockClippingRepository = new Mock<IClippingRepository> { DefaultValue = DefaultValue.Mock };
            _mockEventsHandler = new Mock<IEventsHandler> { DefaultValue = DefaultValue.Mock };
            _mockCallRepository = new Mock<ICallRepository> { DefaultValue = DefaultValue.Mock };
            _mockMessageRepository = new Mock<IMessageRepository> { DefaultValue = DefaultValue.Mock };
            _subject = new EntitySupervisor
                           {
                               ClipboardHandler = _mockClipboardHandler.Object,
                               ClippingRepository = _mockClippingRepository.Object,
                               EventsHandler = _mockEventsHandler.Object,
                               MessageRepository = _mockMessageRepository.Object,
                               CallRepository = _mockCallRepository.Object
                           };
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void OnNewClipping_AfterStart_AlwayStoresClipping()
        {
            const string UniqueId = "42";
            var clipping = new Clipping { UniqueId = UniqueId };
            var clippingObservable = _testScheduler.CreateColdObservable(new Recorded<Notification<Clipping>>(100, Notification.CreateOnNext(clipping)));
            _mockClipboardHandler.Setup(m => m.Subscribe(It.IsAny<IObserver<Clipping>>()))
                .Returns<IObserver<Clipping>>(o => clippingObservable.Subscribe(o));

            _subject.Start();
            _testScheduler.Start(() => clippingObservable);
            
            _mockClippingRepository.Verify(m => m.Save(It.Is<ClippingModel>(c => c.UniqueId == UniqueId)));
        }

        [Test]
        public void OnNewEvent_WhenEventIsIncomingCall_AlwayStoresCall()
        {
            const string UniqueId = "42";
            var @event = new Event { Type = EventTypeEnum.IncomingCallEvent, UniqueId = UniqueId };
            var eventObservable = _testScheduler.CreateColdObservable(new Recorded<Notification<Event>>(100, Notification.CreateOnNext(@event)));
            _mockEventsHandler.Setup(m => m.Subscribe(It.IsAny<IObserver<Event>>()))
                .Returns<IObserver<Event>>(o => eventObservable.Subscribe(o));

            _subject.Start();
            _testScheduler.Start(() => eventObservable);
            
            _mockCallRepository.Verify(m => m.Save(It.Is<Call>(c => c.UniqueId == UniqueId)));
        }

        [Test]
        public void OnNewEvent_WhenEventIsIncomingSms_AlwayStoresMessage()
        {
            const string UniqueId = "42";
            var @event = new Event { Type = EventTypeEnum.IncomingSmsEvent, UniqueId = UniqueId };
            var eventObservable = _testScheduler.CreateColdObservable(new Recorded<Notification<Event>>(100, Notification.CreateOnNext(@event)));
            _mockEventsHandler.Setup(m => m.Subscribe(It.IsAny<IObserver<Event>>()))
                .Returns<IObserver<Event>>(o => eventObservable.Subscribe(o));

            _subject.Start();
            _testScheduler.Start(() => eventObservable);
            
            _mockMessageRepository.Verify(m => m.Save(It.Is<Message>(c => c.UniqueId == UniqueId)));
        }
    }
}
