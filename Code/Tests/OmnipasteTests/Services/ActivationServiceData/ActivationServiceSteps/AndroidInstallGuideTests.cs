namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive;
    using System.Reactive.Subjects;
    using Caliburn.Micro;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class AndroidInstallGuideTests
    {
        private AndroidInstallGuide _subject;

        private ITestableObserver<IExecuteResult> _testableObserver;

        private TestScheduler _testScheduler;

        private Mock<IEventAggregator> _mockEventAggregator;

        private MoqMockingKernel _kernel;

        private IObservable<IExecuteResult> executeObservable;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();
            _mockEventAggregator = _kernel.GetMock<IEventAggregator>();
            _testScheduler = new TestScheduler();
            _testableObserver = _testScheduler.CreateObserver<IExecuteResult>();
            _subject = new AndroidInstallGuide(_mockEventAggregator.Object);
        }

        [Test]
        public void Excute_Always_PublishesShowAndroidInstallGuideMessage()
        {
            _subject.Execute().Subscribe(_testableObserver);

            _mockEventAggregator
                .Verify(ea => ea.Publish(
                    It.IsAny<ShowAndroidInstallGuideMessage>(),
                    It.IsAny<System.Action<System.Action>>()), 
                Times.Once);
        }

        [Test]
        public void Handle_AndroidInstallationComplete_IsSuccessful()
        {
            _subject.Execute().Subscribe(_testableObserver);

            _subject.Handle(new AndroidInstallationCompleteMessage());

            _testableObserver.Messages.Should()
                .Contain(
                    m =>
                        m.Value.Kind == NotificationKind.OnNext && m.Value.Value.State == SimpleStepStateEnum.Successful);
        }
    }
}