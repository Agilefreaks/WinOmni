namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using FluentAssertions;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class GetUserInfoTests
    {
        private GetUserInfo _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new GetUserInfo();
        }

        [Test]
        public void Execute_Always_SetsAUserInfoObjectOnTheResult()
        {
            IExecuteResult result = null;
            _subject.Execute().RunToCompletionSynchronous(newResult => result = newResult);

            result.Data.Should().BeOfType<UserInfo>();
        }
    }
}