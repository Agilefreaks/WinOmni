namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Reactive;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniCommon.Interfaces;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class GetLocalActivationCodeTests
    {
        private GetLocalActivationCode _subject;

        private ITestableObserver<IExecuteResult> _observer;

        private Mock<IConfigurationService> _configurationService;
           
        [SetUp]
        public void SetUp()
        {
            _observer = new TestScheduler().CreateObserver<IExecuteResult>();
            _configurationService = new Mock<IConfigurationService>();
            _subject = new GetLocalActivationCode(_configurationService.Object);
        }

        [Test]
        public void Execute_WhenConfigurationServiceHasAccessToken_WillSucceed()
        {
            _configurationService.SetupGet(m => m.AccessToken).Returns("42");

            _subject.Execute().Subscribe(_observer);

            _observer.Messages.Should()
                .Contain(
                    m =>
                    m.Value.Kind == NotificationKind.OnNext
                    && m.Value.Value.State == SimpleStepStateEnum.Successful
                    && m.Value.Value.Data.GetType() == typeof(Token));
        }

        [Test]
        public void Execute_WhenConfigurationServiceDoesNotHaveAccessToken_WillFail()
        {
            _configurationService.SetupGet(m => m.AccessToken).Returns(string.Empty);

            var observable = _subject.Execute();
            observable.Subscribe(_observer);
            
            observable.Wait();

            _observer.Messages.Should()
                .Contain(
                    m =>
                    m.Value.Kind == NotificationKind.OnNext
                    && m.Value.Value.State == SimpleStepStateEnum.Failed);
        }
    }
};