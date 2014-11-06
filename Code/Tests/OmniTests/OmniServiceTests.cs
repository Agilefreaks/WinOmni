namespace OmniTests
{
    using System;
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omni;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniSync;
    using WampSharp.Auxiliary.Client;

    [TestFixture]
    public class OmniServiceTests
    {
        private readonly string _registrationId = Guid.NewGuid().ToString();

        private const string DeviceName = "Laptop";

        private const string DeviceIdentifier = "42";

        private IOmniService _subject;

        private Mock<IWebsocketConnectionFactory> _websocketConnectionFactory;

        private Mock<IDevices> _mockDevices;

        private Mock<IConfigurationService> _configurationServiceMock;

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
            _configurationServiceMock = _kernel.GetMock<IConfigurationService>();
            _someHandler = _kernel.GetMock<IHandler>();

            _kernel.Bind<IHandler>().ToConstant(_someHandler.Object);

            _configurationServiceMock
                .SetupGet(cs => cs.MachineName)
                .Returns(DeviceName);

            _configurationServiceMock
                .SetupGet(cs => cs.DeviceIdentifier)
                .Returns(DeviceIdentifier);

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
        public void Start_WhenSuccess_ReturnsAUnit()
        {
            var device = new Device { Identifier = DeviceIdentifier };
            var testableObserver = _scheduler.CreateObserver<Unit>();
            var openWebsocketConnection = _scheduler.CreateColdObservable(
                new Recorded<Notification<string>>(0, Notification.CreateOnNext(_registrationId)),
                new Recorded<Notification<string>>(0, Notification.CreateOnCompleted<string>()));
            _mockWebsocketConnection.Setup(m => m.Connect()).Returns(openWebsocketConnection);
            _mockWebsocketConnection.Setup(x => x.SessionId).Returns(_registrationId);
            var registerDevice = _scheduler.CreateColdObservable(
                new Recorded<Notification<Device>>(0, Notification.CreateOnNext(device)),
                new Recorded<Notification<Device>>(0, Notification.CreateOnCompleted<Device>()));
            _mockDevices.Setup(m => m.Create(DeviceIdentifier, DeviceName)).Returns(registerDevice);
            var activateDevice = _scheduler.CreateColdObservable(
                new Recorded<Notification<Device>>(0, Notification.CreateOnNext(device)),
                new Recorded<Notification<Device>>(0, Notification.CreateOnCompleted<Device>()));
            _mockDevices.Setup(m => m.Activate(_registrationId, DeviceIdentifier)).Returns(activateDevice);

            _subject.Start().Subscribe(testableObserver);
            _scheduler.Start();

            testableObserver.Messages.Should().HaveCount(2);
            _subject.State.Should().Be(OmniServiceStatusEnum.Started);
            _someHandler.Verify(m => m.Start(It.IsAny<IWebsocketConnection>()), Times.Once());
        }
    }
}