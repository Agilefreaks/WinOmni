namespace OmniTests
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omni;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
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

        private Mock<IDevices> _devicesMock;

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

            _devicesMock = _kernel.GetMock<IDevices>();
            _configurationServiceMock = _kernel.GetMock<IConfigurationService>();
            _someHandler = _kernel.GetMock<IHandler>();

            _kernel.Bind<IHandler>().ToConstant(_someHandler.Object);

            _configurationServiceMock
                .Setup(cs => cs.MachineName)
                .Returns(DeviceName);

            _configurationServiceMock
                .Setup(cs => cs.DeviceIdentifier)
                .Returns(DeviceIdentifier);

            _mockWebsocketConnection = _kernel.GetMock<IWebsocketConnection>();
            _mockWebsocketConnection.SetupGet(wc => wc.RegistrationId).Returns(_registrationId);

            _scheduler = new TestScheduler();

            ITestableObservable<WebsocketConnectionStatusEnum> testableObservable = _scheduler.CreateColdObservable(new[] { ReactiveTest.OnNext(100, WebsocketConnectionStatusEnum.Disconnected) });
            _mockWebsocketConnection
                .Setup(c => c.Subscribe(It.IsAny<IObserver<WebsocketConnectionStatusEnum>>()))
                .Callback<IObserver<WebsocketConnectionStatusEnum>>(p => testableObservable.Subscribe(p));
            _mockWebsocketConnection.SetupGet(c => c.ConnectionObservable).Returns(testableObservable);

            _websocketConnectionFactory = _kernel.GetMock<IWebsocketConnectionFactory>();
            _websocketConnectionFactory
                .Setup(f => f.Create())
                .Returns(_mockWebsocketConnection.Object);

            _subject = _kernel.Get<IOmniService>();
        }

        [Test]
        public async void Start_RegistersTheDeviceOnTheApi()
        {
            SetupForSuccess();

            await _subject.Start();

            _devicesMock.Verify(api => api.Create(DeviceIdentifier, DeviceName), Times.Once());
            _devicesMock.Verify(api => api.Activate(_registrationId, DeviceIdentifier), Times.Once());
        }

        [Test]
        public async void Start_WhenActivationIsSuccessful_HasStatusStarted()
        {
            SetupForSuccess();

            await _subject.Start();

            _subject.Status.Should().Be(ServiceStatusEnum.Started);
        }

        [Test]
        public async void Start_WhenActivationIsNotSuccessful_HasStatusStopped()
        {
            SetupForFailure();

            await _subject.Start();

            _subject.Status.Should().Be(ServiceStatusEnum.Stopped);
        }

        [Test]
        public async void Start_StartsHandlers()
        {
            SetupForSuccess();

            await _subject.Start();

            _someHandler.Verify(m => m.Start(It.IsAny<IObservable<OmniMessage>>()));
        }

        [Test]
        public async Task Start_SetsTheStatusToStarted()
        {
            SetupForSuccess();

            await _subject.Start();

            Assert.That(_subject.Status, Is.EqualTo(ServiceStatusEnum.Started));
        }

        [Test]
        public async void WhenConnectionIsLostItWillTryToReconnect()
        {
            SetupForSuccess();

            await _subject.Start();
            _scheduler.Start();

            _subject.Status.Should().Be(ServiceStatusEnum.Reconnecting);
        }

        private void SetupForFailure()
        {
            SetupForStart(false);
        }

        private void SetupForSuccess()
        {
            SetupForStart();
        }

        private void SetupForStart(bool success = true)
        {
            var observableCreate = Observable.Create<Device>(
                o =>
                {
                    o.OnNext(new Device());
                    o.OnCompleted();
                    return Disposable.Empty;
                });

            var observableActivate = Observable.Create<Device>(
                o =>
                {
                    o.OnNext(success ? new Device() : null);
                    o.OnCompleted();
                    return Disposable.Empty;
                });

            _devicesMock.Setup(api => api.Create(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(observableCreate);

            _devicesMock
                .Setup(api => api.Activate(_registrationId, DeviceIdentifier))
                .Returns(observableActivate);
        }
    }
}