namespace OmnipasteTests.Services
{
    using System;
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
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

            _mockOmniService.Verify(os => os.Stop(), Times.Once());
        }

        [Test]
        public void LogOut_Calls_ConfigurationServiceWithEmptyValues()
        {
            _subject.LogOut();

            _mockConfigurationService.Verify(cs => cs.ResetAuthSettings(), Times.Once);
        }

        [Test]
        public void LogOut_TriggersOnNextOnSessionDestroyObserver()
        {
            var testableObserver = new TestScheduler().CreateObserver<EventArgs>();
            _subject.SessionDestroyedObservable.Subscribe(testableObserver);

            _subject.LogOut();

            testableObserver.Messages.Should().Contain(m => m.Value.Kind == NotificationKind.OnNext);
        }
    }
}