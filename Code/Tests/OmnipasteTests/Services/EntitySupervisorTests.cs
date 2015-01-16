namespace OmnipasteTests.Services
{
    using System;
    using System.Reactive;
    using Clipboard.Handlers;
    using Clipboard.Models;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using Omnipaste.Services;
    using Omnipaste.Services.Repositories;
    using PhoneCalls.Handlers;
    using PhoneCalls.Models;
    using SMS.Handlers;
    using SMS.Models;

    [TestFixture]
    public class EntitySupervisorTests
    {
        private EntitySupervisor _subject;

        private TestScheduler _testScheduler;

        private Mock<IClipboardHandler> _mockClipboardHandler;

        private Mock<IClippingRepository> _mockClippingRepository;

        private Mock<IPhoneCallReceivedHandler> _mockPhoneCallReceivedHandler;

        private Mock<ISmsMessageCreatedHandler> _mockSmsMessageCreatedHandler;

        private Mock<ICallRepository> _mockCallRepository;

        private Mock<IMessageRepository> _mockMessageRepository;

        private Mock<IUpdaterService> _mockUpdaterService;

        private Mock<IUpdateInfoRepository> _mockUpdateInfoRepository;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
            
            _mockClipboardHandler = new Mock<IClipboardHandler> { DefaultValue = DefaultValue.Mock };
            _mockClippingRepository = new Mock<IClippingRepository> { DefaultValue = DefaultValue.Mock };
            _mockPhoneCallReceivedHandler = new Mock<IPhoneCallReceivedHandler> { DefaultValue = DefaultValue.Mock };
            _mockSmsMessageCreatedHandler = new Mock<ISmsMessageCreatedHandler> { DefaultValue = DefaultValue.Mock };
            _mockCallRepository = new Mock<ICallRepository> { DefaultValue = DefaultValue.Mock };
            _mockMessageRepository = new Mock<IMessageRepository> { DefaultValue = DefaultValue.Mock };
            _mockUpdaterService = new Mock<IUpdaterService> { DefaultValue = DefaultValue.Mock };
            _mockUpdateInfoRepository = new Mock<IUpdateInfoRepository> { DefaultValue = DefaultValue.Mock };
            _subject = new EntitySupervisor
                           {
                               ClipboardHandler = _mockClipboardHandler.Object,
                               ClippingRepository = _mockClippingRepository.Object,
                               PhoneCallReceivedHandler = _mockPhoneCallReceivedHandler.Object,
                               SmsMessageCreatedHandler = _mockSmsMessageCreatedHandler.Object,
                               MessageRepository = _mockMessageRepository.Object,
                               CallRepository = _mockCallRepository.Object,
                               UpdaterService = _mockUpdaterService.Object,
                               UpdateInfoRepository = _mockUpdateInfoRepository.Object
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
            const string Id = "42";
            var clipping = new Clipping { Id = Id };
            var clippingObservable = _testScheduler.CreateColdObservable(new Recorded<Notification<Clipping>>(100, Notification.CreateOnNext(clipping)));
            _mockClipboardHandler.Setup(m => m.Subscribe(It.IsAny<IObserver<Clipping>>()))
                .Returns<IObserver<Clipping>>(o => clippingObservable.Subscribe(o));

            _subject.Start();
            _testScheduler.Start(() => clippingObservable);
            
            _mockClippingRepository.Verify(m => m.Save(It.Is<ClippingModel>(c => c.Id == Id)));
        }

        [Test]
        public void OnNewEvent_WhenEventIsIncomingCall_AlwayStoresCall()
        {
            const string Id = "42";
            var phoneCall = new PhoneCall { Id = Id };
            var phoneCallObservable = _testScheduler.CreateColdObservable(new Recorded<Notification<PhoneCall>>(100, Notification.CreateOnNext(phoneCall)));
            _mockPhoneCallReceivedHandler.Setup(m => m.Subscribe(It.IsAny<IObserver<PhoneCall>>()))
                .Returns<IObserver<PhoneCall>>(o => phoneCallObservable.Subscribe(o));
            
            _subject.Start();
            _testScheduler.Start(() => phoneCallObservable);
            
            _mockCallRepository.Verify(m => m.Save(It.Is<Call>(c => c.Id == Id)));
        }

        [Test]
        public void OnSmsMessageCreated_Alway_AlwayStoresMessage()
        {
            const string Id = "42";
            var smsMessage = new SmsMessage { Id = Id };
            var smsMessageObservable = _testScheduler.CreateColdObservable(new Recorded<Notification<SmsMessage>>(100, Notification.CreateOnNext(smsMessage)));
            _mockSmsMessageCreatedHandler.Setup(m => m.Subscribe(It.IsAny<IObserver<SmsMessage>>()))
                .Returns<IObserver<SmsMessage>>(o => smsMessageObservable.Subscribe(o));

            _subject.Start();
            _testScheduler.Start(() => smsMessageObservable);
            
            _mockMessageRepository.Verify(m => m.Save(It.Is<Message>(c => c.Id == Id)));
        }

        [Test]
        public void OnNewUpdateInfo_Always_SotresUpdateInfo()
        {
            const string UniqueId = "42";
            var updateInfo = new UpdateInfo { UniqueId = UniqueId };
            var updateInfoObservable = _testScheduler.CreateColdObservable(new Recorded<Notification<UpdateInfo>>(100, Notification.CreateOnNext(updateInfo)));
            _mockUpdaterService.SetupGet(m => m.UpdateObservable).Returns(updateInfoObservable);

            _subject.Start();
            _testScheduler.Start(() => updateInfoObservable);
            
            _mockUpdateInfoRepository.Verify(m => m.Save(It.Is<UpdateInfo>(c => c.UniqueId == UniqueId)));
        }
    }
}
