namespace ClipboardTests.Api.Resources.v1
{
    using Clipboard.API.Resources.v1;
    using Clipboard.Dto;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;

    [TestFixture]
    public class ClippingsTests
    {
        private Clippings _subject;

        private Mock<IConfigurationService> _mockConfigurationService;

        private Mock<IWebProxyFactory> _mockWebProxyFactory;

        private Mock<IClippingsApi> _mockResourceApi;

        [SetUp]
        public void SetUp()
        {
            _mockConfigurationService = new Mock<IConfigurationService> { DefaultValue = DefaultValue.Mock };
            _mockWebProxyFactory = new Mock<IWebProxyFactory> { DefaultValue = DefaultValue.Mock };
            _mockResourceApi = new Mock<IClippingsApi> { DefaultValue = DefaultValue.Mock };
            _subject = new Clippings(_mockConfigurationService.Object, _mockWebProxyFactory.Object)
                           {
                               ResourceApi = _mockResourceApi.Object
                           };
        }

        [Test]
        public void Create_Always_CreatesAClippingWithDeviceIdAndContent()
        {
            const string DeviceId = "42";
            const string Content = "test";
            
            _subject.Create(DeviceId, Content);

            _mockResourceApi.Verify(m => m.Create(It.Is<ClippingDto>(c => c.DeviceId == DeviceId && c.Content == Content), It.IsAny<string>()));
        }
    }
}
