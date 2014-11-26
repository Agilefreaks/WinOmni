namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
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
            var autoResetEvent = new AutoResetEvent(false);
            SchedulerProvider.Default = new NewThreadScheduler();

            Task.Factory.StartNew(
                () =>
                    {
                        result = _subject.Execute().Wait();
                        autoResetEvent.Set();
                    });

            autoResetEvent.WaitOne();
            result.Data.Should().BeOfType<UserInfo>();
        }
    }
}