namespace OmnipasteTests.Services.Monitors.Internet
{
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Framework.Services.Monitors.Internet;

    [TestFixture]
    public class InternetConnectivityMonitorTests
    {
        private IInternetConnectivityMonitor _subject;

        private Mock<IConnectivityHelper> _mockConnectivityHelper;

        [SetUp]
        public void SetUp()
        {
            _mockConnectivityHelper = new Mock<IConnectivityHelper>();
            _subject = new InternetConnectivityMonitor(_mockConnectivityHelper.Object);
        }

        [Test]
        public void ConnectivityChangedObservable_AfterCallingStart_GeneratesANewValueForEachTimeTheInternetConnectedValueChanges()
        {
            var testScheduler = new TestScheduler();
            SchedulerProvider.Default = testScheduler;
            var testableObservable =
                testScheduler.CreateColdObservable(
                    new Recorded<Notification<bool>>(100, Notification.CreateOnNext(false)),
                    new Recorded<Notification<bool>>(200, Notification.CreateOnNext(false)),
                    new Recorded<Notification<bool>>(300, Notification.CreateOnNext(true)),
                    new Recorded<Notification<bool>>(400, Notification.CreateOnNext(true)),
                    new Recorded<Notification<bool>>(500, Notification.CreateOnNext(true)),
                    new Recorded<Notification<bool>>(600, Notification.CreateOnNext(false)),
                    new Recorded<Notification<bool>>(700, Notification.CreateOnNext(false)),
                    new Recorded<Notification<bool>>(800, Notification.CreateOnNext(false)));
            _mockConnectivityHelper.SetupGet(x => x.InternetConnectivityObservable)
                .Returns(testableObservable);

            _subject.Start();
            var testableObserver = testScheduler.Start(() => _subject.ConnectivityChangedObservable, 0, 50, 900);

            testableObserver.Messages.Count.Should().Be(2);
        }
    }
}