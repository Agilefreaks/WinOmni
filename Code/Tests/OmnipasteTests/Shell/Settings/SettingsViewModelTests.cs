namespace OmnipasteTests.Shell.Settings
{
    using FluentAssertions;
    using MahApps.Metro.Controls;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using Omnipaste.Shell.Settings;

    [TestFixture]
    public class SettingsViewModelTests
    {
        private ISettingsViewModel _subject;

        private Mock<ISessionManager> _mockSessionManager;

        [SetUp]
        public void SetUp()
        {
            _mockSessionManager = new Mock<ISessionManager>();

            _subject = new SettingsViewModel { SessionManager = _mockSessionManager.Object };

        }

        [Test]
        public void NewInstance_IsNotOpened()
        {
            _subject.IsOpen.Should().BeFalse();
        }

        [Test]
        public void NewInstance_ShouldBePlacedOnTheRight()
        {
            _subject.Position.Should().Be(Position.Right);
        }

        [Test]
        public void LogOut_CallsSessionManagerLogOut()
        {
            _subject.LogOut();

            _mockSessionManager.Verify(mss => mss.LogOut(), Times.Once());
        }

        [Test]
        public void LogOut_ClosesTheFlyout()
        {
            _subject.IsOpen = true;

            _subject.LogOut();

            _subject.IsOpen.Should().BeFalse();
        }
    }
}