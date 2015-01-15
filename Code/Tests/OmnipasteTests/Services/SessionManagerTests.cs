﻿namespace OmnipasteTests.Services
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Interfaces;
    using Omnipaste.Services;

    [TestFixture]
    public class SessionManagerTests
    {
        private Mock<IConfigurationService> _mockConfigurationService;

        private ISessionManager _subject;

        private Mock<IDevices> _mockDevices;

        [SetUp]
        public void SetUp()
        {
            _mockConfigurationService = new Mock<IConfigurationService>();
            _mockDevices = new Mock<IDevices>();
            _mockDevices.Setup(x => x.Remove(It.IsAny<string>())).Returns(Observable.Return(new EmptyModel()));
            _subject = new SessionManager
                       {
                           ConfigurationService = _mockConfigurationService.Object,
                           Devices = _mockDevices.Object
                       };
        }

        [Test]
        public void LogOut_Always_ClearsStoredConfigurationValues()
        {
            _subject.LogOut();

            _mockConfigurationService.Verify(cs => cs.ClearSettings(), Times.Once);
        }

        [Test]
        public void LogOut_TriggersOnNextOnSessionDestroyObserver()
        {
            var testableObserver = new TestScheduler().CreateObserver<EventArgs>();
            _subject.SessionDestroyedObservable.Subscribe(testableObserver);

            _subject.LogOut();

            testableObserver.Messages.Should().Contain(m => m.Value.Kind == NotificationKind.OnNext);
        }

        [Test]
        public void LogOut_Always_RemovesTheCurrentDeviceFromTheUserDeviceList()
        {
             var deviceId = Guid.NewGuid().ToString();
            _mockConfigurationService.SetupGet(x => x.DeviceId).Returns(deviceId);

            _subject.LogOut();

            _mockDevices.Verify(x => x.Remove(deviceId));
        }
    }
}