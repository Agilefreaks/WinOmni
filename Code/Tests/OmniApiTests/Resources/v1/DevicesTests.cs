namespace OmniApiTests.Resources.v1
{
    using System;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Resources.v1;
    using OmniCommon.Interfaces;

    [TestFixture]
    public class DevicesTests
    {
        private Devices _subject;

        private Mock<IConfigurationService> _mockConfigurationService;

        private Mock<IWebProxyFactory> _mockWebProxyFactory;

        private Mock<IDevicesApi> _mockDevicesAPI;

        private const string Accesstoken = "AccessToken";

        private const string Refreshtoken = "RefreshToken";

        private readonly Version _version = new Version("1.2.3.4");

        [SetUp]
        public void Setup()
        {
            _mockConfigurationService = new Mock<IConfigurationService> { DefaultValue = DefaultValue.Mock };
            _mockWebProxyFactory = new Mock<IWebProxyFactory>();
            _mockDevicesAPI = new Mock<IDevicesApi> { DefaultValue = DefaultValue.Mock };
            _subject = new Devices(_mockConfigurationService.Object, _mockWebProxyFactory.Object)
                           {
                               ResourceApi = _mockDevicesAPI.Object
                           };
            _mockConfigurationService.Setup(x => x.AccessToken).Returns(Accesstoken);
            _mockConfigurationService.Setup(x => x.RefreshToken).Returns(Refreshtoken);
            _mockConfigurationService.Setup(x => x.Version).Returns(_version);
        }

        [Test]
        public void Activate_Always_CallsResourceApiPatchWithTheCorrectParameters()
        {
            const string RegistrationId = "someRegistrationId";
            const string DeviceId = "someDeviceId";

            _subject.Activate(RegistrationId, DeviceId);

            //The registration id is required to identify the websocket channel to talk to the device and the provider
            //is required to identify which notification provider is to be used when sending notifications (WebSocket or GoogleCloudMessaging)
            var expectedDeviceParams = new { RegistrationId, Provider = Devices.NotificationsProvider };
            _mockDevicesAPI.Verify(
                x =>
                x.Patch(
                    DeviceId,
                    ObjectWithHashCode(expectedDeviceParams.GetHashCode()),
                    "bearer " + Accesstoken,
                    _version.ToString()));
        }

        [Test]
        public void Deactivate_Always_PatchesTheGivenDeviceWithAnEmptyRegistrationId()
        {
            const string DeviceId = "SomeDeviceId";
            
            _subject.Deactivate(DeviceId);

            var expectedDeviceParams = new { RegistrationId = string.Empty };
            _mockDevicesAPI.Verify(
                x =>
                x.Patch(
                    DeviceId,
                    ObjectWithHashCode(expectedDeviceParams.GetHashCode()),
                    "bearer " + Accesstoken,
                    _version.ToString()));
        }

        [Test]
        public void Update_Always_PatchesTheGivenDeviceWithTheGivenParams()
        {
            const string DeviceId = "SomeDeviceId";
            var toUpdate = new { SomeProperty = "someValue", SomeOtherProperty = "someOtherValue" };

            _subject.Update(DeviceId, toUpdate);

            _mockDevicesAPI.Verify(
                x =>
                x.Patch(
                    DeviceId,
                    ObjectWithHashCode(toUpdate.GetHashCode()),
                    "bearer " + Accesstoken,
                    _version.ToString()));
        }

        private static object ObjectWithHashCode(int expectedHashCode)
        {
            return It.Is<object>(@object => @object.GetHashCode() == expectedHashCode);
        }
    }
}