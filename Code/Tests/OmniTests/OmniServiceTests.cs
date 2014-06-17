namespace OmniTests
{
    using System;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omni;
    using OmniApi.Models;
    using OmniApi.Resources;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using OmniSync;
    using RestSharp;

    [TestFixture]
    public class OmniServiceTests
    {
        private readonly string _registrationId = Guid.NewGuid().ToString();

        private const string DeviceName = "Laptop";
        private const string DeviceIdentifier = "42";

        private IOmniService _subject;

        private ISubject<OmniMessage> _replaySubject;

        private Mock<IWebsocketConnectionFactory> _websocketConnectionFactory;

        private Mock<IDevicesApi> _devicesApiMock;

        private Mock<IConfigurationService> _configurationServiceMock;

        private Mock<IOmniMessageHandler> _mockOmniMessageHandler;

        private Mock<IWebsocketConnection> _mockWebsocketConnection;

        [SetUp]
        public void SetUp()
        {
            var kernel = new MoqMockingKernel();
            kernel.Bind<IOmniService>().To<OmniService>();
            
            _mockOmniMessageHandler = kernel.GetMock<IOmniMessageHandler>();

            _devicesApiMock = kernel.GetMock<IDevicesApi>();
            _configurationServiceMock = kernel.GetMock<IConfigurationService>();
            
            _configurationServiceMock
                .Setup(cs => cs.MachineName)
                .Returns(DeviceName);

            _configurationServiceMock
                .Setup(cs => cs.DeviceIdentifier)
                .Returns(DeviceIdentifier);

            _mockWebsocketConnection = kernel.GetMock<IWebsocketConnection>();
            _mockWebsocketConnection.SetupGet(wc => wc.RegistrationId).Returns(_registrationId);

            _websocketConnectionFactory = kernel.GetMock<IWebsocketConnectionFactory>();
            _websocketConnectionFactory
                .Setup(f => f.Create())
                .Returns(_mockWebsocketConnection.Object);

            _subject = kernel.Get<IOmniService>();
        }

        [Test]
        public async void Start_RegistersTheDeviceOnTheAPI()
        {
            _devicesApiMock.Setup(api => api.Register(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task<IRestResponse<Device>>.Factory.StartNew(() => new RestResponse<Device> { Data = new Device() }));
            _devicesApiMock
                .Setup(api => api.Activate(_registrationId, DeviceIdentifier, It.IsAny<string>()))
                .Returns(Task<IRestResponse<Device>>.Factory.StartNew(() => new RestResponse<Device> { Data = new Device() }));
            
            await _subject.Start();

            _devicesApiMock.Verify(api => api.Register(DeviceIdentifier, DeviceName), Times.Once());
            _devicesApiMock.Verify(api => api.Activate(_registrationId, DeviceIdentifier, It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async void Start_WhenActivationIsSuccessful_HasStatusStarted()
        {
            _devicesApiMock.Setup(api => api.Register(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task<IRestResponse<Device>>.Factory.StartNew(() => new RestResponse<Device> { Data = new Device() }));
            _devicesApiMock
                .Setup(api => api.Activate(_registrationId, DeviceIdentifier, It.IsAny<string>()))
                .Returns(Task<IRestResponse<Device>>.Factory.StartNew(() => new RestResponse<Device> { Data = new Device() })); ;

            await _subject.Start();

            _subject.Status.Should().Be(ServiceStatusEnum.Started);
        }

        [Test]
        public async void Start_WhenActivationIsNotSuccessful_HasStatusStopped()
        {
            _devicesApiMock.Setup(api => api.Register(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task<IRestResponse<Device>>.Factory.StartNew(() => new RestResponse<Device> { Data = new Device() }));
            _devicesApiMock
                .Setup(api => api.Activate(_registrationId, DeviceIdentifier, It.IsAny<string>()))
                .Returns(Task<IRestResponse<Device>>.Factory.StartNew(() => new RestResponse<Device>())); ;

            await _subject.Start();

            _subject.Status.Should().Be(ServiceStatusEnum.Stopped);
        }

        [Test]
        public async void Start_SubscribesMessageHandlers()
        {
            _devicesApiMock.Setup(api => api.Register(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task<IRestResponse<Device>>.Factory.StartNew(() => new RestResponse<Device> { Data = new Device() }));
            _devicesApiMock
                .Setup(api => api.Activate(_registrationId, DeviceIdentifier, It.IsAny<string>()))
                .Returns(Task<IRestResponse<Device>>.Factory.StartNew(() => new RestResponse<Device> { Data = new Device() }));

            await _subject.Start();

            _mockOmniMessageHandler.Verify(omh => omh.SubscribeTo(_mockWebsocketConnection.Object), Times.Once());
        }

        [Test]
        public async Task Start_SetsTheStatusToStarted()
        {
            _devicesApiMock.Setup(api => api.Register(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task<IRestResponse<Device>>.Factory.StartNew(() => new RestResponse<Device> { Data = new Device() }));
            _devicesApiMock
                .Setup(api => api.Activate(_registrationId, DeviceIdentifier, It.IsAny<string>()))
                .Returns(Task<IRestResponse<Device>>.Factory.StartNew(() => new RestResponse<Device> { Data = new Device() }));

            await _subject.Start();

            Assert.That(_subject.Status, Is.EqualTo(ServiceStatusEnum.Started));
        }
    }
}