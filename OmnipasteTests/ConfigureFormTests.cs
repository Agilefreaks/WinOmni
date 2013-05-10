using Moq;
using NUnit.Framework;
using Omniclipboard;
using Omniclipboard.Services;
using Omnipaste;

namespace OmnipasteTests
{
    [TestFixture]
    public class ConfigureFormTests
    {
        ConfigureForm _subject;
        private Mock<IActivationDataProvider> _mockActivationDataProvider;
        private Mock<IConfigurationService> _mockConfigurationService;
        private Mock<IOmniclipboard> _mockOmniclipboard;

        [SetUp]
        public void Setup()
        {
            _mockActivationDataProvider = new Mock<IActivationDataProvider> { DefaultValue = DefaultValue.Mock };
            _mockConfigurationService = new Mock<IConfigurationService>();
            _mockOmniclipboard = new Mock<IOmniclipboard>();
            _subject = new ConfigureForm(_mockActivationDataProvider.Object, _mockConfigurationService.Object, _mockOmniclipboard.Object);
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
            _mockActivationDataProvider.Setup(x => x.GetActivationData()).Returns(new ActivationData { Channel = "testC" });

            _subject.AssureClipboardIsInitialized();

            _mockConfigurationService.Verify(x => x.UpdateCommunicationChannel("testC"));
        }

        [Test]
        public void AssureClipboardIsInitialized_IfTheClipboardInitializationFailsAndMaxRetriesHasNotBeenReached_RetriesToGetTheActivationData()
        {
            _subject.AssureClipboardIsInitialized();

            _mockActivationDataProvider.Verify(x => x.GetActivationData(), Times.Exactly(1 + ConfigureForm.MaxRetryCount));
        }

        [Test]
        public void AssureClipboardIsInitialized_IfTheClipboardInitializationWorks_DoesNotRetryToGetTheActivationData()
        {
            _mockOmniclipboard.Setup(x => x.Initialize()).Returns(true);

            _subject.AssureClipboardIsInitialized();

            _mockActivationDataProvider.Verify(x => x.GetActivationData(), Times.Exactly(1));
        }
    }
}