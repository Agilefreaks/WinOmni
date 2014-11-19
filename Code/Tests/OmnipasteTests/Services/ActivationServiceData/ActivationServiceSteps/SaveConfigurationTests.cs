namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Reactive.Linq;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniCommon.Interfaces;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class SaveConfigurationTests
    {
        private SaveConfiguration _subject;

        private Mock<IConfigurationService> _configurationService;

        [SetUp]
        public void SetUp()
        {
            _configurationService = new Mock<IConfigurationService>();

            _subject = new SaveConfiguration(_configurationService.Object);
        }

        [Test]
        public void Execute_Always_SaveTheConfiguration()
        {
            _subject.Parameter = new DependencyParameter(null, new Token("access token", "refresh token"));
            var observable = _subject.Execute();

            observable.Subscribe(new TestScheduler().CreateObserver<IExecuteResult>());
            observable.Wait();

            _configurationService.Verify(m => m.SaveAuthSettings("access token", "refresh token"));
        }
    }
}