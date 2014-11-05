namespace OmnipasteTests.Services.Connectivity
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Services.Connectivity;

    [TestFixture]
    public class ConnectivityNotifyServiceTests
    {
        private IConnectivityNotifyService _subject;

        private Mock<IConnectivityHelper> _mockConnectivityHelper;

        private TimeSpan _checkInterval;

        [SetUp]
        public void SetUp()
        {
            _mockConnectivityHelper = new Mock<IConnectivityHelper>();
            _checkInterval = TimeSpan.FromMilliseconds(100);
            _subject = new ConnectivityNotifyService(_mockConnectivityHelper.Object, _checkInterval);
        }

        [Test]
        public void ConnectivityChangedObservable_AfterCallingStart_GeneratesANewValueForEachTimeTheInternetConnectedPropertyChanges()
        {
            var values = new [] { false, true, true, true, false };
            var index = 0;
            _mockConnectivityHelper.SetupGet(x => x.InternetConnected).Returns(() => values[index++]);

            var detectedChangeCount = 0;
            _subject.ConnectivityChangedObservable.SubscribeOn(Scheduler.Default)
                .ObserveOn(Scheduler.Default)
                .Subscribe(_ => detectedChangeCount++);
            _subject.Start();

            Thread.Sleep(TimeSpan.FromMilliseconds(_checkInterval.TotalMilliseconds * values.Length));
            detectedChangeCount.Should().Be(2);
        }
    }
}