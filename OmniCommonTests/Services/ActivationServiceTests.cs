namespace OmniCommonTests.Services
{
    using System;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Services;
    using OmniCommon.Services.ActivationServiceData;
    using OmniCommon.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class ActivationServiceTests
    {
        private class ActivationServiceWrapper : ActivationService
        {
            public void CallSetCurrentStep(IActivationStep step)
            {
                SetCurrentStep(step);
            }
        }

        private ActivationServiceWrapper _subject;

        private Mock<IStepFactory> _mockStepFactory;

        private Mock<IActivationStep> _mockActivationStep;

        [SetUp]
        public void Setup()
        {
            _mockStepFactory = new Mock<IStepFactory>();
            _mockActivationStep = new Mock<IActivationStep>();
            _mockActivationStep.Setup(x => x.Execute()).Returns(new ExecuteResult());
            _mockStepFactory.Setup(x => x.Create(It.IsAny<Type>())).Returns(_mockActivationStep.Object);
            _subject = new ActivationServiceWrapper { StepFactory = _mockStepFactory.Object, };
        }

        [Test]
        public void Initialize_ShouldCallStepFactoryCreateWithTypeOfStartAndSetTheResultAsTheCurrentStep()
        {
            var startStep = new Start();
            _mockStepFactory.Setup(x => x.Create(typeof(Start))).Returns(startStep);

            _subject.Initialize();

            _mockStepFactory.Verify(x => x.Create(typeof(Start)), Times.Once());
            _subject.CurrentStep.Should().Be(startStep);
        }

        [Test]
        public void MoveToNextStep_Always_CallsExecuteOnTheCurrentStep()
        {
            _subject.CallSetCurrentStep(_mockActivationStep.Object);

            _subject.MoveToNextStep();

            _mockActivationStep.Verify(x => x.Execute(), Times.Once());
        }

        [Test]
        public void MoveToNextStep_CurrentStepIsStart_ShouldCallStepFactoryCreateWithTypeOfGetTokenFromActivationDataAndSetTheResultAsCurrentStep()
        {
            RunTransitionTest(typeof(Start), SingleStateEnum.Successful, typeof(GetTokenFromActivationData));
        }

        [Test]
        public void MoveToNextStep_CurrentStepIsGetTokenFromActivationDataAndItsStateIsSuccessful_ShouldCallStepFactoryCreateWithGetConfigurationAndSetTheCurrentStep()
        {
            RunTransitionTest(typeof(GetTokenFromActivationData), SimpleStepStateEnum.Successful, typeof(GetConfiguration));
        }

        [Test]
        public void MoveToNextStep_CurrentStepIsGetTokenFromActivationDataAndItsStateIsFailed_ShouldCallStepFactoryCreateWithGetTokenFromUserAndSetTheCurrentStep()
        {
            RunTransitionTest(typeof(GetTokenFromActivationData), SimpleStepStateEnum.Failed, typeof(GetTokenFromUser));
        }

        [Test]
        public void MoveToNextStep_CurrentStepIsGetTokenFromUserAndItsStateIsSuccessful_ShouldCallStepFactoryCreateWithGetConfigurationAndSetTheCurrentStep()
        {
            RunTransitionTest(typeof(GetTokenFromUser), SimpleStepStateEnum.Successful, typeof(GetConfiguration));
        }

        [Test]
        public void MoveToNextStep_CurrentStepIsGetTokenFromUserAndItsStateIsFailed_ShouldCallStepFactoryCreateWithFailedFromUserAndSetTheCurrentStep()
        {
            RunTransitionTest(typeof(GetTokenFromUser), SimpleStepStateEnum.Failed, typeof(Failed));
        }

        [Test]
        public void MoveToNextStep_CurrentStepIsGetConfigurationAndItsStateIsTimedOut_ShouldCallStepFactoryCreateWithGetConfigurationAndSetTheCurrentStep()
        {
            RunTransitionTest(typeof(GetConfiguration), GetConfigurationStepStateEnum.TimedOut, typeof(GetConfiguration));
        }

        [Test]
        public void MoveToNextStep_CurrentStepIsGetConfigurationAndItsStateIsFailed_ShouldCallStepFactoryCreateWithGetTokenFromUserAndSetTheCurrentStep()
        {
            RunTransitionTest(typeof(GetConfiguration), GetConfigurationStepStateEnum.Failed, typeof(GetTokenFromUser));
        }

        [Test]
        public void MoveToNextStep_CurrentStepIsGetConfigurationAndItsStateIsSuccessful_ShouldCallStepFactoryCreateWithFinishedFromUserAndSetTheCurrentStep()
        {
            RunTransitionTest(typeof(GetConfiguration), GetConfigurationStepStateEnum.Successful, typeof(Finished));
        }

        [Test]
        public void MoveToNextStep_CurrentStepIsFinishedAndItsStateIsSuccessful_ShouldCallStepFactoryCreateWithFinishedFromUserAndSetTheCurrentStep()
        {
            RunTransitionTest(typeof(Finished), SingleStateEnum.Successful, typeof(Finished));
        }

        private void RunTransitionTest(object currentStepId, object currentStepState, Type expectedStepType)
        {
            SetCurrentStep(currentStepId, currentStepState);
            var resultStep = new Mock<IActivationStep>().Object;
            _mockStepFactory.Setup(x => x.Create(expectedStepType)).Returns(resultStep);

            _subject.MoveToNextStep();

            _mockStepFactory.Verify(x => x.Create(expectedStepType), Times.Once());
            _subject.CurrentStep.Should().Be(resultStep);
        }

        private void SetCurrentStep(object id, object state, object data = null)
        {
            var mockActivationStep = new Mock<IActivationStep>();
            mockActivationStep.Setup(x => x.GetId()).Returns(id);
            mockActivationStep.Setup(x => x.Execute()).Returns(new ExecuteResult { State = state, Data = data });
            _subject.CallSetCurrentStep(mockActivationStep.Object);
        }
    }
}