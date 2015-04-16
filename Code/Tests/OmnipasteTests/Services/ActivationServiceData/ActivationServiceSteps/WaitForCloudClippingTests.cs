namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Reactive;
    using Clipboard.Dto;
    using Clipboard.Handlers;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class WaitForCloudClippingTests
    {
        private WaitForCloudClipping _subject;

        private Mock<IOmniClipboardHandler> _mockCloudClippingHandler;

        [SetUp]
        public void Setup()
        {
            _mockCloudClippingHandler = new Mock<IOmniClipboardHandler>();
            _subject = new WaitForCloudClipping(_mockCloudClippingHandler.Object);
        }

        [Test]
        public void Execute_WhenSubscribedTo_WaitsForACloudClipping()
        {
            var testScheduler = new TestScheduler();
            SchedulerProvider.Default = testScheduler;
            var cloudClippingObservable =
                testScheduler.CreateColdObservable(
                    new Recorded<Notification<ClippingDto>>(
                        100,
                        Notification.CreateOnNext(new ClippingDto())));
            _mockCloudClippingHandler.Setup(x => x.Clippings).Returns(cloudClippingObservable);

            var testableObserver = testScheduler.Start(_subject.Execute);

            testableObserver.Messages.Count.Should().Be(2);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[0].Value.Value.State.Should().Be(SimpleStepStateEnum.Successful);
            testableObserver.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }
    }
}