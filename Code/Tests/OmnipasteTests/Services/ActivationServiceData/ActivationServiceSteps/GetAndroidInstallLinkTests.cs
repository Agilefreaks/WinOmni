﻿namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;
    using Omnipaste.Framework.Services;
    using Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps;

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
            var userInfo = UserInfo.BeginBuild().WithEmail("test@email.com").Build();
            _mockConfigurationService.SetupGet(x => x.UserInfo).Returns(userInfo);
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
            var userInfo = UserInfo.BeginBuild().WithEmail("test@email.com").Build();
            _mockConfigurationService.SetupGet(x => x.UserInfo).Returns(userInfo);
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