namespace SMSTests.Api.Resources.v1
{
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using SMS.Dto;
    using SMS.Resources.v1;

    [TestFixture]
    public class SmsMessagesTests
    {
        private SMSMessages _subject;

        private Mock<IConfigurationService> _mockConfigurationService;

        private Mock<IWebProxyFactory> _mockWebProxyFactory;

        private Mock<ISMSMessagesApi> _mockResourceApi;

        [SetUp]
        public void SetUp()
        {
            _mockConfigurationService = new Mock<IConfigurationService> { DefaultValue = DefaultValue.Mock };
            _mockWebProxyFactory = new Mock<IWebProxyFactory> { DefaultValue = DefaultValue.Mock };
            _mockResourceApi = new Mock<ISMSMessagesApi> { DefaultValue = DefaultValue.Mock };
            _subject = new SMSMessages(_mockConfigurationService.Object, _mockWebProxyFactory.Object)
                           {
                               ResourceApi = _mockResourceApi.Object
                           };
        }

        [Test]
        public void Send_Always_SendsAnSmsMessage()
        {
            const string PhoneNumber = "1234";
            const string Message = "test";
            const string DeviceId = "42";
            _mockConfigurationService.Setup(m => m.DeviceId).Returns(DeviceId);

            _subject.Send(PhoneNumber, Message);

            _mockResourceApi.Verify(
                m =>
                m.Create(
                    It.Is<SmsMessageDto>(
                        message =>
                        message.PhoneNumber == PhoneNumber && message.Content == Message && message.DeviceId == DeviceId
                        && message.Type == SmsMessageType.Outgoing && message.State == SmsMessageState.Sending),
                    It.IsAny<string>()));
        }
    }
}
