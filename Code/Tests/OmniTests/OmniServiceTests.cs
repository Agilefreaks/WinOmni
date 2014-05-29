using System;
using System.Threading.Tasks;
using Moq;
using OmniApi.Models;
using OmniApi.Resources;
using OmniCommon.Interfaces;
using OmniCommon.Services;
using RestSharp;

namespace OmniTests
{
    using NUnit.Framework;
    using Omni;
    using OmniSync;

    [TestFixture]
    public class OmniServiceTests
    {
        private readonly string _registrationId = Guid.NewGuid().ToString();

        private const string DeviceName = "Laptop";
        private const string DeviceIdentifier = "42";

        private OmniService _subject;

        private Mock<IOmniSyncService> _omniSyncServiceMock;

        private Mock<IDevicesAPI> _devicesApiMock;

        private Mock<IConfigurationService> _configurationServiceMock;

        [SetUp]
        public void SetUp()
        {
            _omniSyncServiceMock = new Mock<IOmniSyncService>();
            _devicesApiMock = new Mock<IDevicesAPI>();
            _configurationServiceMock = new Mock<IConfigurationService>();
            
            _configurationServiceMock
                .Setup(cs => cs.GetMachineName())
                .Returns(DeviceName);

            _configurationServiceMock
                .Setup(cs => cs.GetDeviceIdentifier())
                .Returns(DeviceIdentifier);
            
            _omniSyncServiceMock
                .Setup(os => os.Start())
                .Returns(Task<RegistrationResult>.Factory.StartNew(() => new RegistrationResult { Data = _registrationId }));
            
            _subject = new OmniService(_omniSyncServiceMock.Object, _devicesApiMock.Object, _configurationServiceMock.Object);
        }

        [Test]
        public async void Start_RegistersTheDeviceOnTheAPI()
        {
            _devicesApiMock.Setup(api => api.Register(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task<IRestResponse<Device>>.Factory.StartNew(() => new RestResponse<Device> { Data = new Device() }));
            _devicesApiMock
                .Setup(api => api.Activate(_registrationId, DeviceIdentifier, It.IsAny<string>()))
                .Returns(Task<IRestResponse<Device>>.Factory.StartNew(() => new RestResponse<Device> { Data = new Device() })); ;
            
            await _subject.Start();

            _devicesApiMock.Verify(api => api.Register(DeviceIdentifier, DeviceName), Times.Once());
            _devicesApiMock.Verify(api => api.Activate(_registrationId, DeviceIdentifier, It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async void Start_WhenActivationIsSuccessful_ReturnsTrue()
        {
            _devicesApiMock.Setup(api => api.Register(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task<IRestResponse<Device>>.Factory.StartNew(() => new RestResponse<Device> { Data = new Device() }));
            _devicesApiMock
                .Setup(api => api.Activate(_registrationId, DeviceIdentifier, It.IsAny<string>()))
                .Returns(Task<IRestResponse<Device>>.Factory.StartNew(() => new RestResponse<Device> { Data = new Device() })); ;

            var result = await _subject.Start();

            Assert.IsTrue(result);
        }

        [Test]
        public async void Start_WhenActivationIsNotSuccessful_ReturnsFalse()
        {
            _devicesApiMock.Setup(api => api.Register(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task<IRestResponse<Device>>.Factory.StartNew(() => new RestResponse<Device> { Data = new Device() }));
            _devicesApiMock
                .Setup(api => api.Activate(_registrationId, DeviceIdentifier, It.IsAny<string>()))
                .Returns(Task<IRestResponse<Device>>.Factory.StartNew(() => new RestResponse<Device>())); ;

            var result = await _subject.Start();

            Assert.IsFalse(result);
        }
    }
}