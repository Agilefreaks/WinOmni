namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.DataProviders;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class GetActivationCodeFromDeploymentUriTests
    {
        private GetActivationCodeFromDeploymentUri _subject;

        private ITestableObserver<IExecuteResult> _observer;

        private Mock<IApplicationDeploymentInfoProvider> _applicationDeploymentInfoProvider;

        [SetUp]
        public void SetUp()
        {
            _observer = new TestScheduler().CreateObserver<IExecuteResult>();
            _applicationDeploymentInfoProvider = new Mock<IApplicationDeploymentInfoProvider>();
            _subject = new GetActivationCodeFromDeploymentUri(_applicationDeploymentInfoProvider.Object);
        }

        [Test]
        public void Execute_WhenHasValidActivationUri_ReturnsSuccess()
        {
            _applicationDeploymentInfoProvider.SetupGet(m => m.HasValidActivationUri).Returns(true);
            _applicationDeploymentInfoProvider.SetupGet(m => m.ActivationUri)
                .Returns(new Uri("http://some.com?token=123"));

            _subject.InternalExecute().Subscribe(_observer);

            _observer.Messages.Should()
                .Contain(m => 
                    m.Value.Kind == NotificationKind.OnNext
                    && m.Value.Value.State == SimpleStepStateEnum.Successful);

        }
    }
}