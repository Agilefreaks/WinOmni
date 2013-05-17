using Moq;
using NUnit.Framework;
using OmniCommon.Interfaces;
using OmniCommon.Services;
using Omnipaste;

namespace OmnipasteTests
{
    [TestFixture]
    public class ConfigureFormTests
    {
        private ConfigureForm _subject;
        private Mock<IActivationDataProvider> _mockActivationDataProvider;
        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void Setup()
        {
            _mockActivationDataProvider = new Mock<IActivationDataProvider> { DefaultValue = DefaultValue.Mock };
            _mockConfigurationService = new Mock<IConfigurationService>();
            _subject = new ConfigureForm
                {
                    ActivationDataProvider = _mockActivationDataProvider.Object,
                    ConfigurationService = _mockConfigurationService.Object
                };
        }

        [Test]
        public void AssureClipboardIsInitialized_Always_CallsActivationDataProviderGetActivationData()
        {
            _subject.AssureClipboardIsInitialized();

            _mockActivationDataProvider.Verify(x => x.GetActivationData());
        }

        [Test]
        public void AssureClipboardIsInitialized_Always_CallsUpdateCommunicationChannelWithTheResultOfGetActivationData()
        {
            _mockActivationDataProvider.Setup(x => x.GetActivationData()).Returns(new ActivationData { Email = "testC" });

            _subject.AssureClipboardIsInitialized();

            _mockConfigurationService.Verify(x => x.UpdateCommunicationChannel("testC"));
        }

        [Test]
        public void AssureClipboardIsInitialized_IfTheClipboardInitializationFailsAndMaxRetriesHasNotBeenReached_RetriesToGetTheActivationData()
        {
            _subject.AssureClipboardIsInitialized();

            _mockActivationDataProvider.Verify(x => x.GetActivationData(), Times.Exactly(1 + ConfigureForm.MaxRetryCount));
        }
    }
}