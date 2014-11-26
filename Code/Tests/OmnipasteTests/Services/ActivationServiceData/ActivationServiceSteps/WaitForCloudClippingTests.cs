namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using Omni;
    using OmniCommon.Helpers;
    using OmniCommon.Models;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class WaitForCloudClippingTests
    {
        private WaitForCloudClipping _subject;

        private Mock<IOmniService> _mockOmniService;

        [SetUp]
        public void Setup()
        {
            _mockOmniService = new Mock<IOmniService>();
            _subject = new WaitForCloudClipping(_mockOmniService.Object);
        }

        [Test]
        public void Execute_WhenSubscribedTo_WaitsForAWebsocketClipboardMessage()
        {
            var testScheduler = new TestScheduler();
            SchedulerProvider.Default = testScheduler;
            var omniMessageObservable =
                testScheduler.CreateColdObservable(
                    new Recorded<Notification<OmniMessage>>(
                        100,
                        Notification.CreateOnNext(new OmniMessage(OmniMessageTypeEnum.Notification))),
                    new Recorded<Notification<OmniMessage>>(
                        200,
                        Notification.CreateOnNext(new OmniMessage(OmniMessageTypeEnum.Clipboard))));
            _mockOmniService.SetupGet(x => x.OmniMessageObservable).Returns(omniMessageObservable);

            var testableObserver = testScheduler.Start(_subject.Execute);

            testableObserver.Messages.Count.Should().Be(2);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[0].Value.Value.State.Should().Be(SimpleStepStateEnum.Successful);
            testableObserver.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }
    }
}