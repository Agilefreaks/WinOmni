namespace OmnipasteTests.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Reactive.Linq;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class ResetApplicationStateTests
    {
        private ResetApplicationState _subject;

        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void SetUp()
        {
            _mockConfigurationService = new Mock<IConfigurationService>();

            _subject = new ResetApplicationState(_mockConfigurationService.Object);
        }

        [Test]
        public void Execute_Always_ClearsSettings()
        {
            _subject.Execute().Wait();

            _mockConfigurationService.Verify(m => m.ClearSettings());
        }

        [Test]
        public void Execute_Always_ReturnsSuccess()
        {
            _subject.Execute()
                .Do(result => result.State.Should().Be(SimpleStepStateEnum.Successful))
                .Wait();
        }
    }
}
