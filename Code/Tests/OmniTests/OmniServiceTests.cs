namespace OmniTests
{
    using System;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omni;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using OmniSync;

    [TestFixture]
    public class OmniServiceTests
    {
        private readonly string _registrationId = Guid.NewGuid().ToString();

        private const string DeviceName = "Laptop";

        private const string DeviceIdentifier = "42";

        private IOmniService _subject;

        private Mock<IWebsocketConnectionFactory> _websocketConnectionFactory;

        private Mock<IDevices> _mockDevices;

        private Mock<IConfigurationService> _mockConfigurationService;

        private Mock<IWebsocketConnection> _mockWebsocketConnection;

        private Mock<IHandler> _someHandler;

        private TestScheduler _scheduler;

        private MoqMockingKernel _kernel;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();
            _kernel.Bind<IOmniService>().To<OmniService>();

            _kernel.Bind<IntPtr>().ToConstant(IntPtr.Zero);

            _mockDevices = _kernel.GetMock<IDevices>();
            _mockConfigurationService = _kernel.GetMock<IConfigurationService>();
            _someHandler = _kernel.GetMock<IHandler>();

            _kernel.Bind<IHandler>().ToConstant(_someHandler.Object);

            _mockConfigurationService
                .SetupGet(cs => cs.MachineName)
                .Returns(DeviceName);

            _mockConfigurationService
                .SetupGet(cs => cs.DeviceIdentifier)
                .Returns(DeviceIdentifier);

            _mockConfigurationService.SetupGet(cs => cs.AccessToken).Returns("SomeToken");

            _mockWebsocketConnection = _kernel.GetMock<IWebsocketConnection>();

            _scheduler = new TestScheduler();

            _websocketConnectionFactory = _kernel.GetMock<IWebsocketConnectionFactory>();
            _websocketConnectionFactory
                .Setup(f => f.Create())
                .Returns(_mockWebsocketConnection.Object);

            SchedulerProvider.Default = _scheduler;

            _subject = _kernel.Get<IOmniService>();
        }

        [Test]
        public void Start_WhenAllTasksSucceed_CompletesAfterSendingAUnit()
        {
            SetupOmniServiceForStart();
            var testableObserver = _scheduler.CreateObserver<Unit>();

            _subject.Start().Subscribe(testableObserver);
            _scheduler.Start();

            testableObserver.Messages.Should().HaveCount(2);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }
        
        [Test]
        public void Start_CanActivateDevice_StartsHandlers()
        {
            SetupOmniServiceForStart();
            var testableObserver = _scheduler.CreateObserver<Unit>();

            _subject.Start().Subscribe(testableObserver);
            _scheduler.Start();
            
            _someHandler.Verify(m => m.Start(It.IsAny<IObservable<OmniMessage>>()), Times.Once());
        }
        
        [Test]
        public void Start_WhenAllTasksSucceed_SetsStateToStarted()
        {
            SetupOmniServiceForStart();
            var testableObserver = _scheduler.CreateObserver<Unit>();

            _subject.Start().Subscribe(testableObserver);
            _scheduler.Start();

            _subject.State.Should().Be(OmniServiceStatusEnum.Started);
        }

        [Test]
        public void Start_WhenAllTasksSucceed_IsNotInTransition()
        {
            SetupOmniServiceForStart();
            var testableObserver = _scheduler.CreateObserver<Unit>();

            _subject.Start().Subscribe(testableObserver);
            _scheduler.Start();

            _subject.InTransition.Should().Be(false);
        }

        [Test]
        public void Start_WhenNoAccessTokenIsGiven_ReturnsError()
        {
            _mockConfigurationService.SetupGet(x => x.AccessToken).Returns(string.Empty);
            var testableObserver = _scheduler.CreateObserver<Unit>();
            
            _subject.Start().Subscribe(testableObserver);
            _scheduler.Start();

            testableObserver.Messages.Should().HaveCount(1);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnError);
        }

        [Test]
        public void Stop_AfterBeingStarted_CallsDeactivateDeviceWithTheDeviceId()
        {
            SetupOmniServiceForStart();
            _subject.Start().Subscribe(_scheduler.CreateObserver<Unit>());
            _scheduler.Start();

            var testScheduler = new TestScheduler();
            SchedulerProvider.Default = testScheduler;
            var deactivateObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<Device>>(100, Notification.CreateOnNext(new Device())),
                new Recorded<Notification<Device>>(120, Notification.CreateOnCompleted<Device>()));
            var deviceId = Guid.NewGuid().ToString();
            _mockConfigurationService.SetupGet(x => x.DeviceIdentifier).Returns(deviceId);
            _mockDevices.Setup(x => x.Deactivate(deviceId)).Returns(deactivateObservable);
            var testableObserver = testScheduler.CreateObserver<Unit>();

            _subject.Stop().Subscribe(testableObserver);
            testScheduler.Start();

            _mockDevices.Verify(x => x.Deactivate(deviceId), Times.Once());
        }

        [Test]
        public void Stop_WhenDeactivateFails_IgnoresErrorAndSwitchesToStoppedState()
        {
            SetupOmniServiceForStart();
            _subject.Start().Subscribe(_scheduler.CreateObserver<Unit>());
            _scheduler.Start();

            var testScheduler = new TestScheduler();
            SchedulerProvider.Default = testScheduler;
            var deactivateObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<Device>>(100, Notification.CreateOnError<Device>(new Exception("test"))),
                new Recorded<Notification<Device>>(120, Notification.CreateOnCompleted<Device>()));
            var deviceId = Guid.NewGuid().ToString();
            _mockConfigurationService.SetupGet(x => x.DeviceIdentifier).Returns(deviceId);
            _mockDevices.Setup(x => x.Deactivate(deviceId)).Returns(deactivateObservable);
            var testableObserver = testScheduler.CreateObserver<Unit>();

            _subject.Stop().Subscribe(testableObserver);
            testScheduler.Start();

            _subject.State.Should().Be(OmniServiceStatusEnum.Stopped);
            _subject.InTransition.Should().BeFalse();
        }

        [Test]
        public void Dispose_ServiceStoppedAndATransitionIsNotInProgress_WillReplaceTheStatusChangedObservableWithANullSubject()
        {
            SchedulerProvider.Default = Scheduler.Default;

            _subject.Dispose();

            _subject.StatusChangedObservable.Should().BeOfType<NullSubject<OmniServiceStatusEnum>>();
        }

        [Test]
        public void Dispose_ServiceNotStoppedAndATransitionIsNotInProgress_WillReplaceTheStatusChangedObservableWithANullSubject()
        {
            SetupOmniServiceForStart();
            _scheduler.Start(_subject.Start);

            SchedulerProvider.Default = Scheduler.Default;
            _mockDevices.Setup(x => x.Deactivate(It.IsAny<string>())).Returns(Observable.Return(new Device()));

            _subject.Dispose();

            _someHandler.Verify(x => x.Stop());
        }

        [Test]
        public void Dispose_ServiceNotStoppedAndATransitionIsNotInProgress_WillDeactivateTheCurrentDevice()
        {
            SetupOmniServiceForStart();
            _scheduler.Start(_subject.Start);

            SchedulerProvider.Default = Scheduler.Default;
            _mockDevices.Setup(x => x.Deactivate(It.IsAny<string>())).Returns(Observable.Return(new Device()));

            _subject.Dispose();

            _mockDevices.Verify(x => x.Deactivate(_mockConfigurationService.Object.DeviceIdentifier));
        }

        [Test]
        public void Dispose_ServiceNotStoppedAndATransitionIsInProgress_WillNotChangeTheStatusChangedObservable()
        {
            _subject.Start().SubscribeAndHandleErrors();

            _subject.Dispose();

            _subject.StatusChangedObservable.Should().BeOfType<ReplaySubject<OmniServiceStatusEnum>>();
        }

        [Test]
        public void Dispose_ServiceNotStoppedAndATransitionIsInProgress_WillNotStopHandlers()
        {
            _subject.Start().SubscribeAndHandleErrors();

            _subject.Dispose();

            _someHandler.Verify(x => x.Stop(), Times.Never());
        }

        [Test]
        public void Dispose_ServiceNotStoppedAndATransitionIsInProgress_WillNotDeactivateTheDevice()
        {
            _subject.Start().SubscribeAndHandleErrors();

            _subject.Dispose();

            _someHandler.Verify(x => x.Stop(), Times.Never());
        }

        private void SetupOmniServiceForStart()
        {
            var device = new Device { Identifier = DeviceIdentifier };
            var openWebsocketConnection =
                _scheduler.CreateColdObservable(
                    new Recorded<Notification<string>>(0, Notification.CreateOnNext(_registrationId)),
                    new Recorded<Notification<string>>(0, Notification.CreateOnCompleted<string>()));
            _mockWebsocketConnection.Setup(m => m.Connect()).Returns(openWebsocketConnection);
            _mockWebsocketConnection.Setup(x => x.SessionId).Returns(_registrationId);
            var registerDevice =
                _scheduler.CreateColdObservable(
                    new Recorded<Notification<Device>>(0, Notification.CreateOnNext(device)),
                    new Recorded<Notification<Device>>(0, Notification.CreateOnCompleted<Device>()));
            _mockDevices.Setup(m => m.Create(DeviceIdentifier, DeviceName)).Returns(registerDevice);
            var activateDevice =
                _scheduler.CreateColdObservable(
                    new Recorded<Notification<Device>>(0, Notification.CreateOnNext(device)),
                    new Recorded<Notification<Device>>(0, Notification.CreateOnCompleted<Device>()));
            _mockDevices.Setup(m => m.Activate(_registrationId, DeviceIdentifier)).Returns(activateDevice);
        }
    }
}