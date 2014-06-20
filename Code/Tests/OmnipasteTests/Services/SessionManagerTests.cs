namespace OmnipasteTests.Services
{
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Omni;
    using OmniCommon.Interfaces;
    using Omnipaste.Services;

    [TestFixture]
    public class SessionManagerTests
    {
        private Mock<IOmniService> _mockOmniService;

        private Mock<IConfigurationService> _mockConfigurationService;

        private ISessionManager _subject;

        [SetUp]
        public void SetUp()
        {
            _mockOmniService = new Mock<IOmniService>();
            _mockConfigurationService = new Mock<IConfigurationService>();
            _subject = new SessionManager
                       {
                           OmniService = _mockOmniService.Object,
                           ConfigurationService = _mockConfigurationService.Object
                       };
        }

        [Test]
        public void LogOut_CallsOmniServiceStop()
        {
            _subject.LogOut();

            _mockOmniService.Verify(os => os.Stop(true), Times.Once());
        }

        [Test]
        public void LogOut_Calls_ConfigurationServiceWithEmptyValues()
        {
            _subject.LogOut();

            _mockConfigurationService.Verify(cs => cs.SaveAuthSettings(string.Empty, string.Empty, string.Empty), Times.Once);
        }

        [Test]
        public void LogOut_TriggersSessionDestroyedEvent()
        {
            bool wasCalled = false;
            _subject.SessionDestroyed += (o, e) => wasCalled = true;

            _subject.LogOut();

            wasCalled.Should().BeTrue();
        }
    }
}