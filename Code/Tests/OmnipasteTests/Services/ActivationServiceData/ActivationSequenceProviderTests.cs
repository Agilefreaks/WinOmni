namespace OmnipasteTests.Services.ActivationServiceData
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    [TestFixture]
    public class ActivationSequenceProviderTests
    {
        private ActivationSequenceProvider _subject;

        private ActivationSequence _sequence;

        [SetUp]
        public void Setup()
        {
            _subject = new ActivationSequenceProvider();
            _sequence = _subject.Get();
        }

        [Test]
        public void LoadLocalConfiguration_Success_ShouldBeStartOmniService()
        {
            _sequence.Transitions.GetTargetTypeForTransition<GetLocalActivationCode>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<StartOmniService>();
        }

        [Test]
        public void LoadLocalConfiguration_Failure_ShouldBeGetActivationCodeFromArguments()
        {
            _sequence.Transitions.GetTargetTypeForTransition<GetLocalActivationCode>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<GetActivationCodeFromArguments>();
        }

        [Test]
        public void GetActivationCodeFromArguments_Success_ShouldBeGetRemoteConfiguration()
        {
            _sequence.Transitions.GetTargetTypeForTransition<GetActivationCodeFromArguments>(
                SimpleStepStateEnum.Successful).Should().Be<GetRemoteConfiguration>();
        }

        [Test]
        public void GetActivationCodeFromArguments_Failed_ShouldBeGetActivationCodeFromUser()
        {
            _sequence.Transitions.GetTargetTypeForTransition<GetActivationCodeFromArguments>(
                SimpleStepStateEnum.Failed).Should().Be<GetActivationCodeFromUser>();
        }

        [Test]
        public void GetActivationCodeFromUser_Success_ShouldBeGetRemoteConfiguration()
        {
            _sequence.Transitions.GetTargetTypeForTransition<GetActivationCodeFromUser>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<GetRemoteConfiguration>();
        }

        [Test]
        public void GetActivationCodeFromUser_Failed_ShouldBeGetActivationCodeFromUser()
        {
            _sequence.Transitions.GetTargetTypeForTransition<GetActivationCodeFromUser>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<GetActivationCodeFromUser>();
        }

        [Test]
        public void GetRemoteConfiguration_Success_ShouldBeFinished()
        {
            _sequence.Transitions.GetTargetTypeForTransition<GetRemoteConfiguration>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<SaveConfiguration>();
        }

        [Test]
        public void GetRemoteConfiguration_Failed_ShouldBeGetActivationCodeFromUser()
        {
            _sequence.Transitions.GetTargetTypeForTransition<GetRemoteConfiguration>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<GetActivationCodeFromUser>();
        }

        [Test]
        public void SaveConfiguration_Failed_ShouldBeFailed()
        {
            _sequence.Transitions.GetTargetTypeForTransition<SaveConfiguration>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<Failed>();
        }

        [Test]
        public void SaveConfiguration_Success_ShouldBeStartOmniService()
        {
            _sequence.Transitions.GetTargetTypeForTransition<SaveConfiguration>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<StartOmniService>();
        }

        [Test]
        public void StartOmniService_Failed_ShouldBeFailed()
        {
            _sequence.Transitions.GetTargetTypeForTransition<StartOmniService>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<Failed>();
        }

        [Test]
        public void StartOmniService_Success_ShouldBeVerifyNumberOfDevices()
        {
            _sequence.Transitions.GetTargetTypeForTransition<StartOmniService>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<VerifyNumberOfDevices>();
        }

        [Test]
        public void VerifyNumberOfDevices_OnFailed_ShouldBeGetUserInfo()
        {
            _sequence.Transitions.GetTargetTypeForTransition<VerifyNumberOfDevices>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<GetUserInfo>();
        }

        [Test]
        public void VerifyNumberOfDevices_OnSuccess_ShouldBeFinished()
        {
            _sequence.Transitions.GetTargetTypeForTransition<VerifyNumberOfDevices>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<Finished>();
        }

        [Test]
        public void GetUserInfo_OnFailed_ShouldBeFailed()
        {
            _sequence.Transitions.GetTargetTypeForTransition<GetUserInfo>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<Failed>();
        }

        [Test]
        public void GetUserInfo_OnSuccess_ShouldBeGetAndroidInstallLink()
        {
            _sequence.Transitions.GetTargetTypeForTransition<GetUserInfo>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<GetAndroidInstallLink>();
        }

        [Test]
        public void GetAndroidInstallLink_OnFail_ShouldBeFailed()
        {
            _sequence.Transitions.GetTargetTypeForTransition<GetAndroidInstallLink>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<Failed>();
        }

        [Test]
        public void GetAndroidInstallLink_OnSuccessful_ShouldBeAndroidInstallGuide()
        {
            _sequence.Transitions.GetTargetTypeForTransition<GetAndroidInstallLink>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<AndroidInstallGuide>();
        }

        [Test]
        public void AndroidInstallGuide_OnFail_ShouldBeFailed()
        {
            _sequence.Transitions.GetTargetTypeForTransition<AndroidInstallGuide>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<Failed>();
        }
        
        [Test]
        public void AndroidInstallGuide_OnSuccess_ShouldBeWaitForSecondDevice()
        {
            _sequence.Transitions.GetTargetTypeForTransition<AndroidInstallGuide>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<WaitForSecondDevice>();
        }

        [Test]
        public void WaitForSecondDevice_OnFail_ShouldBeFailed()
        {
            _sequence.Transitions.GetTargetTypeForTransition<WaitForSecondDevice>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<Failed>();
        }

        [Test]
        public void WaitForSecondDevice_OnSuccess_ShouldBeShowCongratulations()
        {
            _sequence.Transitions.GetTargetTypeForTransition<WaitForSecondDevice>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<ShowCongratulations>();
        }

        [Test]
        public void ShowCongratulations_OnFail_ShouldBeFailed()
        {
            _sequence.Transitions.GetTargetTypeForTransition<ShowCongratulations>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<Failed>();
        }
        
        [Test]
        public void ShowCongratulations_OnSuccess_ShouldBeWaitForCloudClipping()
        {
            _sequence.Transitions.GetTargetTypeForTransition<ShowCongratulations>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<WaitForCloudClipping>();
        }
        
        [Test]
        public void WaitForCloudClipping_OnFail_ShouldBeFailed()
        {
            _sequence.Transitions.GetTargetTypeForTransition<WaitForCloudClipping>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<Failed>();
        }
        
        [Test]
        public void WaitForCloudClipping_OnSuccess_ShouldBeFinished()
        {
            _sequence.Transitions.GetTargetTypeForTransition<WaitForCloudClipping>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<Finished>();
        }

        [Test]
        public void VerifyConnectivity_OnSuccess_ShouldTransitionToGetLocalActivationCode()
        {
            _sequence.Transitions.GetTargetTypeForTransition<VerifyConnectivity>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<GetLocalActivationCode>();
        }

        [Test]
        public void VerifyConnectivity_OnFail_ShouldTransitionToFixProxyConfiguration()
        {
            _sequence.Transitions.GetTargetTypeForTransition<VerifyConnectivity>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<FixProxyConfiguration>();
        }

        [Test]
        public void FixProxyConfiguration_OnSuccess_ShouldTransitionToVerifyConnectivity()
        {
            _sequence.Transitions.GetTargetTypeForTransition<FixProxyConfiguration>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<VerifyConnectivity>();
        }

        [Test]
        public void FixProxyConfiguration_OnFail_ShouldTransitionToShowConnectionTroubleshooter()
        {
            _sequence.Transitions.GetTargetTypeForTransition<FixProxyConfiguration>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<ShowConnectionTroubleshooter>();
        }

        [Test]
        public void ShowConnectionTroubleshooter_OnSuccess_ShouldTransitionToVerifyConnectivity()
        {
            _sequence.Transitions.GetTargetTypeForTransition<FixProxyConfiguration>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<VerifyConnectivity>();
        }

        [Test]
        public void ShowConnectionTroubleshooter_OnFail_ShouldTransitionToVerifyConnectivity()
        {
            _sequence.Transitions.GetTargetTypeForTransition<FixProxyConfiguration>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<ShowConnectionTroubleshooter>();
        }

        [Test]
        public void InitialStepId_Always_IsVerifyConnectivity()
        {
            _sequence.InitialStepId.Should().Be<VerifyConnectivity>();
        }

        [Test]
        public void FinalStepIds_Always_ContainsFinished()
        {
            _sequence.FinalStepIdIds.Should().Contain(typeof(Finished));
        }

        [Test]
        public void FinalStepIds_Always_ContainsFailed()
        {
            _sequence.FinalStepIdIds.Should().Contain(typeof(Failed));
        }
    }
}