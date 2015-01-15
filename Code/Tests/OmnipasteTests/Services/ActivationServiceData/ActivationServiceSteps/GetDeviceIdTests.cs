namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class GetDeviceIdTests
    {
        private GetDeviceId _subject;

        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void Setup()
        {
            _mockConfigurationService = new Mock<IConfigurationService>();
            _subject = new GetDeviceId(_mockConfigurationService.Object);
        }

        [Test]
        public void Execute_AtEndAndConfigurationServiceHasNoDeviceId_ReturnsFailedResult()
        {
            var autoResetEvent = new AutoResetEvent(false);
            _mockConfigurationService.Setup(x => x.DeviceIdentifier).Returns((string)null);
            SchedulerProvider.Default = new NewThreadScheduler();

            IExecuteResult executeResult = null;
            Task.Factory.StartNew(
                () =>
                {
                    executeResult = _subject.Execute().Wait();
                    autoResetEvent.Set();
                });

            autoResetEvent.WaitOne();
            executeResult.State.Should().Be(SimpleStepStateEnum.Failed);
        }

        [Test]
        public void Execute_AtEndAndConfigurationServiceHasADeviceId_ReturnsSuccessfulResult()
        {
            var autoResetEvent = new AutoResetEvent(false);
            _mockConfigurationService.Setup(x => x.DeviceId).Returns("test");
            SchedulerProvider.Default = new NewThreadScheduler();

            IExecuteResult executeResult = null;
            Task.Factory.StartNew(
                () =>
                {
                    executeResult = _subject.Execute().Wait();
                    autoResetEvent.Set();
                });

            autoResetEvent.WaitOne();
            executeResult.State.Should().Be(SimpleStepStateEnum.Successful);
        }
    }
}