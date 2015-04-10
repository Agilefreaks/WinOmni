namespace OmnipasteTests.Shell.TitleBar
{
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Entities;
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
        public void OnUpdateReceived_WhenWasInstalledIsFalse_UpdatesCanPerformActionToTrue()
        {
            var updateEntity = new UpdateEntity { WasInstalled = false };
            var updateObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<UpdateEntity>>(100, Notification.CreateOnNext(updateEntity)),
                    new Recorded<Notification<UpdateEntity>>(200, Notification.CreateOnCompleted<UpdateEntity>()));
            _mockUpdaterService.SetupGet(m => m.UpdateObservable).Returns(updateObservable);
            _subject = CreateSubject();

            _testScheduler.Start();

            _subject.CanPerformAction.Should().BeTrue();
        }

        [Test]
        public void OnUpdateReceived_WhenWasInstalledIsTrue_UpdatesCanPerformActionToFalse()
        {
            var updateEntity = new UpdateEntity { WasInstalled = true };
            var updateObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<UpdateEntity>>(100, Notification.CreateOnNext(updateEntity)),
                    new Recorded<Notification<UpdateEntity>>(200, Notification.CreateOnCompleted<UpdateEntity>()));
            _mockUpdaterService.SetupGet(m => m.UpdateObservable).Returns(updateObservable);
            _subject = CreateSubject();

            _testScheduler.Start();

            _subject.CanPerformAction.Should().BeFalse();
        }

        private NewVersionTitleBarItemViewModel CreateSubject()
        {
            return new NewVersionTitleBarItemViewModel(_mockUpdaterService.Object);
        }
    }
}