using FluentAssertions;
using Moq;
using NUnit.Framework;
using PubNubClipboard.Services;

namespace PubNubClipboardTests.Services
{
    [TestFixture]
    public class ConfigurationServiceTests
    {
        private Mock<IConfigurationProvider> _mockConfigurationProvider;
        private ConfigurationService _subject;

        [SetUp]
        public void Setup()
        {
            _mockConfigurationProvider = new Mock<IConfigurationProvider>();
            _subject = new ConfigurationService(_mockConfigurationProvider.Object);
        }

        [Test]
        public void Startup_Should_LoadCommunicationSettings()
        {
            _mockConfigurationProvider.Setup(x => x.GetValue(It.IsAny<string>())).Returns("testResult");

            _subject.Startup();

            _subject.CommunicationSettings.Should().NotBeNull();
            _subject.CommunicationSettings.Channel.Should().Be("testResult");
        }

        [Test]
        public void UpdateCommunicationChannel_Always_ShouldSetTheChannelValueToTheGivenValue()
        {
            _subject.UpdateCommunicationChannel("testChannel");

            _mockConfigurationProvider.Verify(x => x.SetValue("channel", "testChannel"));
        }

        [Test]
        public void UpdateCommunicationChannel_Always_ShouldReloadTheCommunicationSettings()
        {
            _mockConfigurationProvider.Setup(x => x.GetValue(It.IsAny<string>())).Returns("testChannel");

            _subject.UpdateCommunicationChannel("testChannel");

            _subject.CommunicationSettings.Should().NotBeNull();
            _subject.CommunicationSettings.Channel.Should().Be("testChannel");
        }
    }
}