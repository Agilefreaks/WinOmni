namespace OmnipasteTests.Services
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.Services;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

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
            this._mockStepFactory = new Mock<IStepFactory>();
            this._mockActivationStep = new Mock<IActivationStep>();
            this._mockActivationStep.Setup(x => x.Execute()).Returns(new ExecuteResult());
            this._mockStepFactory.Setup(x => x.Create(It.IsAny<Type>(), null)).Returns(this._mockActivationStep.Object);
            this._subject = new ActivationServiceWrapper(this._mockStepFactory.Object);
        }

        [Test]
        public void GetNextStepCurrentStepIsNullReturnsAStartStep()
        {
            this._subject.CallSetCurrentStep(null);

            this._subject.GetNextStep().Should().BeOfType<Start>();
        }

        [Test]
        public void GetNextStepCurrentStepNotNullCallsExecuteOnTheCurrentStep()
        {
            this._subject.CallSetCurrentStep(this._mockActivationStep.Object);

            this._subject.MoveToNextStep();

            this._mockActivationStep.Verify(x => x.Execute(), Times.Once());
        }

        [Test]
        public void GetNextStepCurrentStepIsStartShouldCallStepFactoryCreateWithTypeOfGetTokenFromDeploymentUriAndSetTheResultAsCurrentStep()
        {
            this.RunTransitionTest(typeof(Start), SingleStateEnum.Successful, typeof(GetTokenFromDeploymentUri));
        }

        [Test]
        public void GetNextStepCurrentStepIsGetTokenFromDeploymentUriAndItsStateIsSuccessfulShouldCallStepFactoryCreateWithGetConfigurationAndSetTheCurrentStep()
        {
            this.RunTransitionTest(typeof(GetTokenFromDeploymentUri), SimpleStepStateEnum.Successful, typeof(GetRemoteConfiguration));
        }

        [Test]
        public void GetNextStepCurrentStepIsGetTokenFromDeploymentUriAndItsStateIsFailedShouldCallStepFactoryCreateWithLoadLocalConfigurationAndSetTheCurrentStep()
        {
            this.RunTransitionTest(typeof(GetTokenFromDeploymentUri), SimpleStepStateEnum.Failed, typeof(LoadLocalConfiguration));
        }

        [Test]
        public void GetNextStepCurrentStepIsLoadLocalConfigurationAndItsStateIsSuccessfulShouldCallStepFactoryCreateWithFinishedAndSetTheCurrentStep()
        {
            this.RunTransitionTest(typeof(LoadLocalConfiguration), SimpleStepStateEnum.Successful, typeof(Finished));
        }

        [Test]
        public void GetNextStepCurrentStepIsLoadLocalConfigurationAndItsStateIsSuccessfulShouldCallStepFactoryCreateWithGetTokenFromUserAndSetTheCurrentStep()
        {
            this.RunTransitionTest(typeof(LoadLocalConfiguration), SimpleStepStateEnum.Failed, typeof(GetTokenFromUser));
        }

        [Test]
        public void GetNextStepCurrentStepIsGetTokenFromUserAndItsStateIsSuccessfulShouldCallStepFactoryCreateWithGetRemoteConfigurationAndSetTheCurrentStep()
        {
            this.RunTransitionTest(typeof(GetTokenFromUser), SimpleStepStateEnum.Successful, typeof(GetRemoteConfiguration));
        }

        [Test]
        public void GetNextStepCurrentStepIsGetTokenFromUserAndItsStateIsFailedShouldCallStepFactoryCreateWithFailedFromUserAndSetTheCurrentStep()
        {
            this.RunTransitionTest(typeof(GetTokenFromUser), SimpleStepStateEnum.Failed, typeof(Failed));
        }

        [Test]
        public void GetNextStepCurrentStepIsGetRemoteConfigurationAndItsStateIsCommunicationFailureShouldCallStepFactoryCreateWithGetConfigurationAndSetTheCurrentStep()
        {
            this.RunTransitionTest(typeof(GetRemoteConfiguration), GetRemoteConfigurationStepStateEnum.CommunicationFailure, typeof(GetRemoteConfiguration));
        }

        [Test]
        public void GetNextStepCurrentStepIsGetRemoteConfigurationAndItsStateIsFailedShouldCallStepFactoryCreateWithGetTokenFromUserAndSetTheCurrentStep()
        {
            this.RunTransitionTest(typeof(GetRemoteConfiguration), GetRemoteConfigurationStepStateEnum.Failed, typeof(GetTokenFromUser));
        }

        [Test]
        public void GetNextStepCurrentStepIsGetRemoteConfigurationAndItsStateIsSuccessfulShouldCallStepFactoryCreateWithFinishedFromUserAndSetTheCurrentStep()
        {
            this.RunTransitionTest(typeof(GetRemoteConfiguration), GetRemoteConfigurationStepStateEnum.Successful, typeof(SaveConfiguration));
        }

        [Test]
        public void GetNextStepCurrentStepIsSaveConfigurationAndItsStateIsSuccessfulShouldCallStepFactoryCreateWithAndSetTheCurrentStep()
        {
            this.RunTransitionTest(typeof(SaveConfiguration), SingleStateEnum.Successful, typeof(Finished));
        }

        [Test]
        public void GetNextStepCurrentStepIsFinishedAndItsStateIsSuccessfulShouldCallStepFactoryCreateWithFinishedFromUserAndSetTheCurrentStep()
        {
            this.RunTransitionTest(typeof(Finished), SingleStateEnum.Successful, typeof(Finished));
        }

        [Test]
        public void GetNextStepCurrentStepIsFailedAndItsStateIsSuccessfulShouldCallStepFactoryCreateWithFailedFromUserAndSetTheCurrentStep()
        {
            this.RunTransitionTest(typeof(Failed), SingleStateEnum.Successful, typeof(Failed));
        }

        [Test]
        public void FinalStepsAlwaysShouldIncludeFinished()
        {
            this._subject.FinalStepIds.ToList().Contains(typeof(Finished)).Should().BeTrue();
        }

        [Test]
        public void FinalStepsAlwaysShouldIncludeFailed()
        {
            this._subject.FinalStepIds.ToList().Contains(typeof(Failed)).Should().BeTrue();
        }

        [Test]
        public void RunAlwaysShouldMoveToTheNextStepUntillCurrentStepIsAFinalStep()
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
            this._mockStepFactory.Setup(x => x.Create(It.IsAny<Type>(), It.IsAny<object>())).Returns(getStepFunc);

            this._subject.Run();

            callCount.Should().Be(7);
            this._subject.CurrentStep.Should().Be(mockActivationStep.Object);
        }

        private void RunTransitionTest(object currentStepId, object currentStepState, Type expectedStepType)
        {
            this.SetCurrentStep(currentStepId, currentStepState);
            var resultStep = new Mock<IActivationStep>().Object;
            this._mockStepFactory.Setup(x => x.Create(expectedStepType, null)).Returns(resultStep);

            var activationStep = this._subject.GetNextStep();

            this._mockStepFactory.Verify(x => x.Create(expectedStepType, null), Times.Once());
            activationStep.Should().Be(resultStep);
        }

        private void SetCurrentStep(object id, object state, object data = null)
        {
            var mockActivationStep = new Mock<IActivationStep>();
            mockActivationStep.Setup(x => x.GetId()).Returns(id);
            mockActivationStep.Setup(x => x.Execute()).Returns(new ExecuteResult { State = state, Data = data });
            this._subject.CallSetCurrentStep(mockActivationStep.Object);
        }
    }
}