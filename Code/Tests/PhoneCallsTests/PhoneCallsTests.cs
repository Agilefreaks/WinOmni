namespace PhoneCallsTests
{
    using Moq;
    using OmniCommon.Interfaces;
    using PhoneCalls.Models;
    using PhoneCalls.Resources.v1;
   using NUnit.Framework;

    [TestFixture]
    public class PhoneCallsTests
    {
        private PhoneCalls _subject;

        private Mock<IConfigurationService> _mockConfigurationService;

        private Mock<IWebProxyFactory> _mockWebProxyFactory;

        private Mock<IPhoneCallsApi> _mockPhoneCallsAPI;

        private const string AccessToken = "SomeToken";

        [SetUp]
        public void Setup()
        {
            _mockConfigurationService = new Mock<IConfigurationService> { DefaultValue = DefaultValue.Mock };
            _mockWebProxyFactory = new Mock<IWebProxyFactory>();
            _mockPhoneCallsAPI = new Mock<IPhoneCallsApi>();
            _mockConfigurationService.Setup(x => x.AccessToken).Returns(AccessToken);
            _subject = new PhoneCalls(_mockConfigurationService.Object, _mockWebProxyFactory.Object)
                           {
                               ResourceApi = _mockPhoneCallsAPI.Object
                           };
        }

        [Test]
        public void Call_Always_CreatesACallWithTheCorrectProperties()
        {
            const string PhoneNumber = "1231231";
            
            _subject.Call(PhoneNumber);

            _mockPhoneCallsAPI.Verify(
                x =>
                x.Create(
                    It.Is<PhoneCall>(
                        phoneCall =>
                        phoneCall.Number == PhoneNumber && phoneCall.State == PhoneCallState.Starting
                        && phoneCall.Type == PhoneCallType.Outgoing),
                    "bearer SomeToken"));
        }

        [Test]
        public void EndCall_Always_PatchesTheGivenCallWithTheStateEnding()
        {
            const string CallId = "someCallId";
            const string DeviceId = "42";
            _mockConfigurationService.Setup(m => m.DeviceId).Returns(DeviceId);

            _subject.EndCall(CallId);

            var expectedPayload = new { DeviceId = DeviceId, State = PhoneCallState.Ending };
            _mockPhoneCallsAPI.Verify(
                x =>
                x.Patch(
                    CallId,
                    It.Is<object>(payload => payload.GetHashCode() == expectedPayload.GetHashCode()),
                    "bearer SomeToken"));
        }
    }
}
