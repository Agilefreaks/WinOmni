namespace OmnipasteTests.Services.Monitors.Power
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Microsoft.Win32;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Services.Monitors.Power;

    [TestFixture]
    public class PowerMonitorTests
    {
        private PowerMonitor _subject;

        private Mock<ISystemPowerHelper> _mockSystemPowerHelper;

        [SetUp]
        public void Setup()
        {
            _mockSystemPowerHelper = new Mock<ISystemPowerHelper>();
            _subject = new PowerMonitor(_mockSystemPowerHelper.Object);
        }

        [Test]
        public void Start_Always_StartsForwardingEventsFromTheSystemPowerChangedObservableToPowerModesObservable()
        {
            var testScheduler = new TestScheduler();
            SchedulerProvider.Default = testScheduler;
            var powerModeObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<PowerModeChangedEventArgs>>(
                    TimeSpan.FromMilliseconds(100).Ticks,
                    Notification.CreateOnNext(new PowerModeChangedEventArgs(PowerModes.Suspend))),
                new Recorded<Notification<PowerModeChangedEventArgs>>(
                    TimeSpan.FromMilliseconds(200).Ticks,
                    Notification.CreateOnNext(new PowerModeChangedEventArgs(PowerModes.Suspend))),
                new Recorded<Notification<PowerModeChangedEventArgs>>(
                    TimeSpan.FromMilliseconds(400).Ticks,
                    Notification.CreateOnNext(new PowerModeChangedEventArgs(PowerModes.StatusChange))),
                new Recorded<Notification<PowerModeChangedEventArgs>>(
                    TimeSpan.FromMilliseconds(1200).Ticks,
                    Notification.CreateOnNext(new PowerModeChangedEventArgs(PowerModes.Resume))),
                new Recorded<Notification<PowerModeChangedEventArgs>>(
                    TimeSpan.FromMilliseconds(2300).Ticks,
                    Notification.CreateOnNext(new PowerModeChangedEventArgs(PowerModes.StatusChange))),
                new Recorded<Notification<PowerModeChangedEventArgs>>(
                    TimeSpan.FromMilliseconds(3200).Ticks,
                    Notification.CreateOnNext(new PowerModeChangedEventArgs(PowerModes.Resume))),
                new Recorded<Notification<PowerModeChangedEventArgs>>(
                    TimeSpan.FromMilliseconds(3300).Ticks,
                    Notification.CreateOnCompleted<PowerModeChangedEventArgs>()));
            _mockSystemPowerHelper.Setup(x => x.PowerModeChangedObservable).Returns(powerModeObservable);

            var testableObserver = testScheduler.Start(
                () =>
                    {
                        _subject.Start();
                        return _subject.PowerModesObservable;
                    },
                0,
                0,
                TimeSpan.FromMilliseconds(4000).Ticks);

            testableObserver.Messages.Count.Should().Be(5);
            testableObserver.Messages[0].Value.Value.Should().Be(PowerModes.Suspend);
            testableObserver.Messages[1].Value.Value.Should().Be(PowerModes.Suspend);
            testableObserver.Messages[2].Value.Value.Should().Be(PowerModes.Resume);
            testableObserver.Messages[3].Value.Value.Should().Be(PowerModes.Resume);
        }
    }
}