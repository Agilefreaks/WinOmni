namespace OmnipasteTests.Loading
{
    using System;
    using Caliburn.Micro;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Loading.ActivationFailed;
    using Action = System.Action;

    [TestFixture]
    public class ActivationFailedViewModelTests
    {
        private IActivationFailedViewModel _subject;

        private Mock<IApplicationService> _applicationWrapper;

        [SetUp]
        public void SetUp()
        {
            _applicationWrapper = new Mock<IApplicationService>();
            _subject = new ActivationFailedViewModel(_applicationWrapper.Object);
        }

        [Test]
        public void Exit_Always_CallsShutDown()
        {
            _subject.Exit();

            _applicationWrapper.Verify(m => m.ShutDown(), Times.Once);
        }

        [Test]
        public void Retry_Always_PublishesRetryMessage()
        {
            var mockEventAggregator = new Mock<IEventAggregator>();
            _subject.EventAggregator = mockEventAggregator.Object;
            
            _subject.Retry();

            mockEventAggregator.Verify(m => m.Publish(It.IsAny<RetryMessage>(), It.IsAny<Action<Action>>()));
        }
    }
}