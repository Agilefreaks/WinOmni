namespace OmniCommonTests.Services
{
    using System;
    using System.Linq;
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
            public ActivationServiceWrapper(IStepFactory stepFactory)
                : base(stepFactory)
            {
            }

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
            _mockStepFactory.Setup(x => x.Create(It.IsAny<Type>(), null)).Returns(_mockActivationStep.Object);
            _subject = new ActivationServiceWrapper(_mockStepFactory.Object);
        }

        [Test]
        public void GetNextStep_CurrentStepIsNull_ReturnsAStartStep()
        {
            _subject.CallSetCurrentStep(null);

            _subject.GetNextStep().Should().BeOfType<Start>();
        }

        [Test]
        public void GetNextStep_CurrentStepNotNull_CallsExecuteOnTheCurrentStep()
        {
            _subject.CallSetCurrentStep(_mockActivationStep.Object);

            _subject.MoveToNextStep();

            _mockActivationStep.Verify(x => x.Execute(), Times.Once());
        }

        [Test]
        public void GetNextStep_CurrentStepIsStart_ShouldCallStepFactoryCreateWithTypeOfGetTokenFromDeploymentUriAndSetTheResultAsCurrentStep()
        {
            RunTransitionTest(typeof(Start), SingleStateEnum.Successful, typeof(GetTokenFromDeploymentUri));
        }

        [Test]
        public void GetNextStep_CurrentStepIsGetTokenFromDeploymentUriAndItsStateIsSuccessful_ShouldCallStepFactoryCreateWithGetConfigurationAndSetTheCurrentStep()
        {
            RunTransitionTest(typeof(GetTokenFromDeploymentUri), SimpleStepStateEnum.Successful, typeof(GetRemoteConfiguration));
        }

        [Test]
        public void GetNextStep_CurrentStepIsGetTokenFromDeploymentUriAndItsStateIsFailed_ShouldCallStepFactoryCreateWithLoadLocalConfigurationAndSetTheCurrentStep()
        {
            RunTransitionTest(typeof(GetTokenFromDeploymentUri), SimpleStepStateEnum.Failed, typeof(LoadLocalConfiguration));
        }

        [Test]
        public void GetNextStep_CurrentStepIsLoadLocalConfigurationAndItsStateIsSuccessful_ShouldCallStepFactoryCreateWithFinishedAndSetTheCurrentStep()
        {
            RunTransitionTest(typeof(LoadLocalConfiguration), SimpleStepStateEnum.Successful, typeof(Finished));
        }

        [Test]
        public void GetNextStep_CurrentStepIsLoadLocalConfigurationAndItsStateIsSuccessful_ShouldCallStepFactoryCreateWithGetTokenFromUserAndSetTheCurrentStep()
        {
            RunTransitionTest(typeof(LoadLocalConfiguration), SimpleStepStateEnum.Failed, typeof(GetTokenFromUser));
        }

        [Test]
        public void GetNextStep_CurrentStepIsGetTokenFromUserAndItsStateIsSuccessful_ShouldCallStepFactoryCreateWithGetRemoteConfigurationAndSetTheCurrentStep()
        {
            RunTransitionTest(typeof(GetTokenFromUser), SimpleStepStateEnum.Successful, typeof(GetRemoteConfiguration));
        }

        [Test]
        public void GetNextStep_CurrentStepIsGetTokenFromUserAndItsStateIsFailed_ShouldCallStepFactoryCreateWithFailedFromUserAndSetTheCurrentStep()
        {
            RunTransitionTest(typeof(GetTokenFromUser), SimpleStepStateEnum.Failed, typeof(Failed));
        }

        [Test]
        public void GetNextStep_CurrentStepIsGetRemoteConfigurationAndItsStateIsCommunicationFailure_ShouldCallStepFactoryCreateWithGetConfigurationAndSetTheCurrentStep()
        {
            RunTransitionTest(typeof(GetRemoteConfiguration), GetRemoteConfigurationStepStateEnum.CommunicationFailure, typeof(GetRemoteConfiguration));
        }

        [Test]
        public void GetNextStep_CurrentStepIsGetRemoteConfigurationAndItsStateIsFailed_ShouldCallStepFactoryCreateWithGetTokenFromUserAndSetTheCurrentStep()
        {
            RunTransitionTest(typeof(GetRemoteConfiguration), GetRemoteConfigurationStepStateEnum.Failed, typeof(GetTokenFromUser));
        }

        [Test]
        public void GetNextStep_CurrentStepIsGetRemoteConfigurationAndItsStateIsSuccessful_ShouldCallStepFactoryCreateWithFinishedFromUserAndSetTheCurrentStep()
        {
            RunTransitionTest(typeof(GetRemoteConfiguration), GetRemoteConfigurationStepStateEnum.Successful, typeof(SaveConfiguration));
        }

        [Test]
        public void GetNextStep_CurrentStepIsSaveConfigurationAndItsStateIsSuccessful_ShouldCallStepFactoryCreateWithAndSetTheCurrentStep()
        {
            RunTransitionTest(typeof(SaveConfiguration), SingleStateEnum.Successful, typeof(Finished));
        }

        [Test]
        public void GetNextStep_CurrentStepIsFinishedAndItsStateIsSuccessful_ShouldCallStepFactoryCreateWithFinishedFromUserAndSetTheCurrentStep()
        {
            RunTransitionTest(typeof(Finished), SingleStateEnum.Successful, typeof(Finished));
        }

        [Test]
        public void GetNextStep_CurrentStepIsFailedAndItsStateIsSuccessful_ShouldCallStepFactoryCreateWithFailedFromUserAndSetTheCurrentStep()
        {
            RunTransitionTest(typeof(Failed), SingleStateEnum.Successful, typeof(Failed));
        }

        [Test]
        public void FinalSteps_Always_ShouldIncludeFinished()
        {
            _subject.FinalStepIds.ToList().Contains(typeof(Finished)).Should().BeTrue();
        }

        [Test]
        public void FinalSteps_Always_ShouldIncludeFailed()
        {
            _subject.FinalStepIds.ToList().Contains(typeof(Failed)).Should().BeTrue();
        }

        [Test]
        public void Run_Always_ShouldMoveToTheNextStepUntillCurrentStepIsAFinalStep()
        {
            var mockActivationStep = new Mock<IActivationStep>();
            mockActivationStep.Setup(x => x.Execute())
                              .Returns(new ExecuteResult { State = SimpleStepStateEnum.Successful });
            var callCount = 0;
            Func<Type, object, IActivationStep> getStepFunc = (type, payload) =>
                {
                    var idToReturn = callCount == 6 ? typeof(Finished) : typeof(Start);
                    mockActivationStep.Setup(x => x.GetId()).Returns(idToReturn);
                    callCount++;

                    return mockActivationStep.Object;
                };
            _mockStepFactory.Setup(x => x.Create(It.IsAny<Type>(), It.IsAny<object>())).Returns(getStepFunc);

            _subject.Run();

            callCount.Should().Be(7);
            _subject.CurrentStep.Should().Be(mockActivationStep.Object);
        }

        private void RunTransitionTest(object currentStepId, object currentStepState, Type expectedStepType)
        {
            SetCurrentStep(currentStepId, currentStepState);
            var resultStep = new Mock<IActivationStep>().Object;
            _mockStepFactory.Setup(x => x.Create(expectedStepType, null)).Returns(resultStep);

            var activationStep = _subject.GetNextStep();

            _mockStepFactory.Verify(x => x.Create(expectedStepType, null), Times.Once());
            activationStep.Should().Be(resultStep);
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