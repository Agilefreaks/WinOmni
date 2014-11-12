namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Services;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class VerifyConnectivityTests
    {
        private VerifyConnectivity _subject;

        private Mock<INetworkService> _mockVerifyConnectivity;

        [SetUp]
        public void Setup()
        {
            _mockVerifyConnectivity = new Mock<INetworkService>();
            _subject = new VerifyConnectivity(_mockVerifyConnectivity.Object);
        }

        [Test]
        public void Execute_CannotPingHome_ReturnsFailedResult()
        {
            _mockVerifyConnectivity.Setup(x => x.CanPingHome(null)).Returns(false);
            var testScheduler = new TestScheduler();
            var testableObserver = testScheduler.CreateObserver<IExecuteResult>();

            _subject.Execute().SubscribeOn(testScheduler).Subscribe(testableObserver);
            testScheduler.Start();

            testableObserver.Messages.Count.Should().Be(2);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
            testableObserver.Messages[0].Value.Value.State.Should().Be(SimpleStepStateEnum.Failed);
        }

        [Test]
        public void Execute_CanPingHome_ReturnsSuccessfulResult()
        {
            _mockVerifyConnectivity.Setup(x => x.CanPingHome(null)).Returns(true);

            var executeResult = _subject.Execute().Wait();

            executeResult.State.Should().Be(SimpleStepStateEnum.Successful);
        }

        [Test]
        public void Execute_PingHomeThrowsException_CallsOnError()
        {
            var exception = new Exception("test");
            _mockVerifyConnectivity.Setup(x => x.CanPingHome(null)).Throws(exception);

            Action action = () => _subject.Execute().Wait();
            action.ShouldThrow<Exception>().And.Equals(exception);
        }
    }
}