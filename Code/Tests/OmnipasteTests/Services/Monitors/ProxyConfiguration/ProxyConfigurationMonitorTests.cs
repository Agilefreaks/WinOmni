namespace OmnipasteTests.Services.Monitors.ProxyConfiguration
{
    using System;
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using OmniCommon.Settings;
    using Omnipaste.Services.Monitors.ProxyConfiguration;

    [TestFixture]
    public class ProxyConfigurationMonitorTests
    {
        private ProxyConfigurationMonitor _subject;

        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void Setup()
        {
            _mockConfigurationService = new Mock<IConfigurationService>();
            _subject = new ProxyConfigurationMonitor(_mockConfigurationService.Object);
        }

        [Test]
        public void Start_Always_EmitsAProxyConfigurationValueWhenTheProxyConfigurationChanges()
        {
            var testScheduler = new TestScheduler();
            SchedulerProvider.Default = testScheduler;
            var proxyConfiguration = new ProxyConfiguration();
            var settingsObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<SettingsChangedData>>(
                    100,
                    Notification.CreateOnNext(new SettingsChangedData("SomeProperty", new object()))),
                new Recorded<Notification<SettingsChangedData>>(
                    200,
                    Notification.CreateOnNext(
                        new SettingsChangedData(ConfigurationProperties.ProxyConfiguration, proxyConfiguration))));
            _mockConfigurationService.Setup(x => x.SettingsChangedObservable).Returns(settingsObservable);

            _subject.Start();
            var testableObserver = testScheduler.Start(() => _subject.SettingObservable, TimeSpan.FromSeconds(1).Ticks);

            testableObserver.Messages.Count.Should().Be(1);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[0].Value.Value.Should().Be(proxyConfiguration);
        }

        [Test]
        public void Start_WhenCalledMultipleTimes_DoesNotEmitMultipleProxyConfigurationValuesWhenAProxyConfigurationChangeOccurs()
        {
            var testScheduler = new TestScheduler();
            SchedulerProvider.Default = testScheduler;
            var proxyConfiguration = new ProxyConfiguration();
            var settingsObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<SettingsChangedData>>(
                    100,
                    Notification.CreateOnNext(new SettingsChangedData("SomeProperty", new object()))),
                new Recorded<Notification<SettingsChangedData>>(
                    200,
                    Notification.CreateOnNext(
                        new SettingsChangedData(ConfigurationProperties.ProxyConfiguration, proxyConfiguration))));
            _mockConfigurationService.Setup(x => x.SettingsChangedObservable).Returns(settingsObservable);

            _subject.Start();
            _subject.Start();
            var testableObserver = testScheduler.Start(() => _subject.SettingObservable, TimeSpan.FromSeconds(1).Ticks);

            testableObserver.Messages.Count.Should().Be(1);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[0].Value.Value.Should().Be(proxyConfiguration);
        }

        [Test]
        public void Stop_Always_PreventsTheMonitorFromEmittingValuesWhenAProxyConfigurationChangeOccurs()
        {
            var testScheduler = new TestScheduler();
            SchedulerProvider.Default = testScheduler;
            var proxyConfiguration = new ProxyConfiguration();
            var settingsObservable = testScheduler.CreateColdObservable(
                new Recorded<Notification<SettingsChangedData>>(
                    200,
                    Notification.CreateOnNext(
                        new SettingsChangedData(ConfigurationProperties.ProxyConfiguration, proxyConfiguration))));
            _mockConfigurationService.Setup(x => x.SettingsChangedObservable).Returns(settingsObservable);
            _subject.Start();

            _subject.Stop();
            var testableObserver = testScheduler.Start(() => _subject.SettingObservable, TimeSpan.FromSeconds(1).Ticks);

            testableObserver.Messages.Count.Should().Be(0);
        }
    }
}