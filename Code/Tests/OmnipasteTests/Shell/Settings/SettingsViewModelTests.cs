namespace OmnipasteTests.Shell.Settings
{
    using FluentAssertions;
    using MahApps.Metro.Controls;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using Omnipaste.Framework;
    using Omnipaste.Shell.Settings;

    [TestFixture]
    public class SettingsViewModelTests
    {
        private ISettingsViewModel _subject;

        private Mock<ISessionManager> _mockSessionManager;

        private Mock<IApplicationService> _mockApplicationWrapper;

        [SetUp]
        public void SetUp()
        {
            _mockSessionManager = new Mock<ISessionManager>();
            _mockApplicationWrapper = new Mock<IApplicationService>();

            _subject = new SettingsViewModel
                       {
                           SessionManager = _mockSessionManager.Object,
                           ApplicationService = _mockApplicationWrapper.Object
                       };
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

        [Test]
        public void Exit_CallsApplicationShutdown()
        {
            _subject.Exit();

            _mockApplicationWrapper.Verify(aw => aw.ShutDown(), Times.Once);
        }
    }
}