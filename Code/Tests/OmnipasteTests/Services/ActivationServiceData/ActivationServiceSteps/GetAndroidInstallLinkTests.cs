namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Interfaces;
    using Omnipaste.Services;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class GetAndroidInstallLinkTests
    {
        private GetAndroidInstallLink _subject;

        private Mock<IUrlShortenerService> _mockUrlShortenerService;

        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void Setup()
        {
            _mockUrlShortenerService = new Mock<IUrlShortenerService>();
            _mockConfigurationService = new Mock<IConfigurationService>();
            _mockConfigurationService.SetupGet(x => x.WebBaseUrl).Returns("http://a.b");
            _subject = new GetAndroidInstallLink(_mockUrlShortenerService.Object, _mockConfigurationService.Object);
        }

        [Test]
        public void Execute_UrlShortenerServiceFails_SetsANormalUriOnTheResult()
        {
            var userInfo = new User { Email = "test@email.com" };
            _subject.Parameter = new DependencyParameter("test", userInfo);
            var taskCompletionSource = new TaskCompletionSource<Uri>();
            taskCompletionSource.SetException(new Exception("test"));
            _mockUrlShortenerService.Setup(x => x.Shorten(It.IsAny<Uri>())).Returns(taskCompletionSource.Task);

            IExecuteResult result = null;
            _subject.Execute().RunToCompletionSynchronous(newResult => result = newResult);

            result.Data.Should().Be(new Uri("http://a.b/downloads/android_client?email=test%40email.com"));
        }

        [Test]
        public void Execute_UrlShortenerSucceeds_SetsTheObtainedUriOnTheResult()
        {
            var userInfo = new User { Email = "test@email.com" };
            _subject.Parameter = new DependencyParameter("test", userInfo);
            var taskCompletionSource = new TaskCompletionSource<Uri>();
            var shortUrl = new Uri("http://google.com");
            taskCompletionSource.SetResult(shortUrl);
            _mockUrlShortenerService.Setup(x => x.Shorten(It.IsAny<Uri>())).Returns(taskCompletionSource.Task);

            IExecuteResult result = null;
            _subject.Execute().RunToCompletionSynchronous(newResult => result = newResult);

            result.Data.Should().Be(shortUrl);
        }
    }
}