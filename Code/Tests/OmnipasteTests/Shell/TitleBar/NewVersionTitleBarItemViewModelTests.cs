namespace OmnipasteTests.Shell.TitleBar
{
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Services;
    using Omnipaste.Shell.TitleBar;

    [TestFixture]
    public class NewVersionTitleBarItemViewModelTests
    {
        private Mock<IUpdaterService> _mockUpdaterService;

        private NewVersionTitleBarItemViewModel _subject;

        private TestScheduler _testScheduler;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;

            _mockUpdaterService = new Mock<IUpdaterService> { DefaultValue = DefaultValue.Mock };

            _subject = CreateSubject();
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void OnUpdateInfoReceived_WhenWasInstalledIsFalse_UpdatesCanPerformActionToTrue()
        {
            var updateInfo = new UpdateInfo { WasInstalled = false };
            var updateObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<UpdateInfo>>(100, Notification.CreateOnNext(updateInfo)),
                    new Recorded<Notification<UpdateInfo>>(200, Notification.CreateOnCompleted<UpdateInfo>()));
            _mockUpdaterService.SetupGet(m => m.UpdateObservable).Returns(updateObservable);
            _subject = CreateSubject();

            _testScheduler.Start();

            _subject.CanPerformAction.Should().BeTrue();
        }

        [Test]
        public void OnUpdateInfoReceived_WhenWasInstalledIsTrue_UpdatesCanPerformActionToFalse()
        {
            var updateInfo = new UpdateInfo { WasInstalled = true };
            var updateObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<UpdateInfo>>(100, Notification.CreateOnNext(updateInfo)),
                    new Recorded<Notification<UpdateInfo>>(200, Notification.CreateOnCompleted<UpdateInfo>()));
            _mockUpdaterService.SetupGet(m => m.UpdateObservable).Returns(updateObservable);
            _subject = CreateSubject();

            _testScheduler.Start();

            _subject.CanPerformAction.Should().BeTrue();
        }

        private NewVersionTitleBarItemViewModel CreateSubject()
        {
            return new NewVersionTitleBarItemViewModel(_mockUpdaterService.Object);
        }
    }
}