namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class GetAndroidInstallLinkTests
    {
        private GetAndroidInstallLink _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new GetAndroidInstallLink();
        }

        [Test]
        public void Execute_OnSuccess_SetsAUriOnTheResult()
        {
            IExecuteResult result = null;
            var userInfo = new UserInfo { Email = "test@email.com" };
            _subject.Parameter = new DependencyParameter("test", userInfo);

            _subject.Execute().RunToCompletionSynchronous(newResult => result = newResult);

            (result.Data is Uri).Should().BeTrue();
        }
    }
}