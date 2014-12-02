﻿namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Reactive;
    using Clipboard.Handlers;
    using Clipboard.Models;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

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
                    new Recorded<Notification<Clipping>>(
                        100,
                        Notification.CreateOnNext(new Clipping())));
            _mockCloudClippingHandler.Setup(x => x.Clippings).Returns(cloudClippingObservable);

            var testableObserver = testScheduler.Start(_subject.Execute);

            testableObserver.Messages.Count.Should().Be(2);
            testableObserver.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObserver.Messages[0].Value.Value.State.Should().Be(SimpleStepStateEnum.Successful);
            testableObserver.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }
    }
}