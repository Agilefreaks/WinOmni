namespace OmnipasteTests.Services
{
    using FluentAssertions;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.Services;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class ActivationServiceTests
    {
        private MoqMockingKernel _mockingKernel;

        private Mock<IStepFactory> _mockStepFactory;

        private IActivationService _subject;

        [SetUp]
        public void Setup()
        {
            _mockingKernel = new MoqMockingKernel();
            
            _mockStepFactory = new Mock<IStepFactory>();
            _mockingKernel.Bind<IStepFactory>().ToConstant(_mockStepFactory.Object);

            _mockingKernel.Bind<IActivationService>().To<ActivationService>();

            _subject = _mockingKernel.Get<IActivationService>();
        }

        [Test]
        public void Success_WithCurrentStepNull_WillReturnFalse()
        {
            _subject.Success.Should().BeFalse();
        }

        [Test]
        public void LoadLocalConfiguration_Success_ShouldBeFinished()
        {
            _subject.Transitions.GetTargetTypeForTransition<LoadLocalConfiguration>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<Finished>();
        }

        [Test]
        public void LoadLocalConfiguration_Failure_ShouldBeGetActivationCodeFromDeploymentUri()
        {
            _subject.Transitions.GetTargetTypeForTransition<LoadLocalConfiguration>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<GetActivationCodeFromDeploymentUri>();
        }

    }
}