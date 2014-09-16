﻿namespace OmnipasteTests.Services
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
        public void LoadLocalConfiguration_Success_ShouldBeStartOmniService()
        {
            _subject.Transitions.GetTargetTypeForTransition<LoadLocalConfiguration>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<StartOmniService>();
        }

        [Test]
        public void LoadLocalConfiguration_Failure_ShouldBeGetActivationCodeFromArguments()
        {
            _subject.Transitions.GetTargetTypeForTransition<LoadLocalConfiguration>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<GetActivationCodeFromArguments>();
        }

        [Test]
        public void GetActivationCodeFromArguments_Success_ShouldBeGetRemoteConfiguration()
        {
            _subject.Transitions.GetTargetTypeForTransition<GetActivationCodeFromArguments>(
                SimpleStepStateEnum.Successful).Should().Be<GetRemoteConfiguration>();
        }

        [Test]
        public void GetActivationCodeFromArguments_Failed_ShouldBeGetActivationCodeFromUser()
        {
            _subject.Transitions.GetTargetTypeForTransition<GetActivationCodeFromArguments>(
                SimpleStepStateEnum.Failed).Should().Be<GetActivationCodeFromUser>();
        }

        [Test]
        public void GetActivationCodeFromUser_Success_ShouldBeGetRemoteConfiguration()
        {
            _subject.Transitions.GetTargetTypeForTransition<GetActivationCodeFromUser>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<GetRemoteConfiguration>();
        }

        [Test]
        public void GetActivationCodeFromUser_Failed_ShouldBeGetActivationCodeFromUser()
        {
            _subject.Transitions.GetTargetTypeForTransition<GetActivationCodeFromUser>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<GetActivationCodeFromUser>();
        }

        [Test]
        public void GetRemoteConfiguration_Success_ShouldBeFinished()
        {
            _subject.Transitions.GetTargetTypeForTransition<GetRemoteConfiguration>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<SaveConfiguration>();
        }

        [Test]
        public void GetRemoteConfiguration_Failed_ShouldBeGetActivationCodeFromUser()
        {
            _subject.Transitions.GetTargetTypeForTransition<GetRemoteConfiguration>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<GetActivationCodeFromUser>();
        }

        [Test]
        public void SaveConfiguration_Failed_ShouldBeFailed()
        {
            _subject.Transitions.GetTargetTypeForTransition<SaveConfiguration>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<Failed>();
        }

        [Test]
        public void SaveConfiguration_Success_ShouldBeStartOmniService()
        {
            _subject.Transitions.GetTargetTypeForTransition<SaveConfiguration>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<StartOmniService>();
        }

        [Test]
        public void StartOmniService_Failed_ShouldBeFailed()
        {
            _subject.Transitions.GetTargetTypeForTransition<StartOmniService>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<Failed>();            
        }

        [Test]
        public void StartOmniService_Success_ShouldBeVerifyNumberOfDevices()
        {
            _subject.Transitions.GetTargetTypeForTransition<StartOmniService>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<VerifyNumberOfDevices>();
        }

        [Test]
        public void VerifyNumberOfDevices_OnSuccess_ShouldBeAndroidInstallGuide()
        {
            _subject.Transitions.GetTargetTypeForTransition<VerifyNumberOfDevices>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<AndroidInstallGuide>();
        }

        [Test]
        public void VerifyNumberOfDevices_OnSuccess_ShouldBeFinished()
        {
            _subject.Transitions.GetTargetTypeForTransition<VerifyNumberOfDevices>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<Finished>();
        }
    }
}