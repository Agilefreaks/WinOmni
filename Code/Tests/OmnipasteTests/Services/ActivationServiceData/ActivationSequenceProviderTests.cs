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
        public void GetLocalActivationCode_Success_ShouldBeGetDeviceId()
        {
            _sequence.Transitions.GetTargetTypeForTransition<GetLocalActivationCode>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<GetDeviceId>();
        }

        [Test]
        public void GetLocalActivationCode_Failure_ShouldBeGetActivationCodeFromArguments()
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
        public void SaveConfiguration_Success_ShouldBeGetDeviceId()
        {
            _sequence.Transitions.GetTargetTypeForTransition<SaveConfiguration>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<GetDeviceId>();
        }

        [Test]
        public void GetUser_OnFailed_ShouldBeFailed()
        {
            _sequence.Transitions.GetTargetTypeForTransition<GetUser>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<Failed>();
        }

        [Test]
        public void GetUser_OnSuccess_ShouldBeSaveUser()
        {
            _sequence.Transitions.GetTargetTypeForTransition<GetUser>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<SaveUser>();
        }

        [Test]
        public void SaveUser_OnSuccess_ShouldBeStartOmniService()
        {
            _sequence.Transitions.GetTargetTypeForTransition<SaveUser>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<InitDeviceInfos>();
        }

        [Test]
        public void SaveUser_OnFailed_ShouldBeFailed()
        {
            _sequence.Transitions.GetTargetTypeForTransition<SaveUser>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<Failed>();
        }

        [Test]
        public void InitDeviceInfos_OnSuccess_ShouldBeStartOmniService()
        {
            _sequence.Transitions.GetTargetTypeForTransition<InitDeviceInfos>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<StartOmniService>();
        }

        [Test]
        public void InitDeviceInfos_OnFailed_ShouldBeFailed()
        {
            _sequence.Transitions.GetTargetTypeForTransition<InitDeviceInfos>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<Failed>();
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
        public void GetDeviceId_Failed_ShouldBeRegisterDevice()
        {
            _sequence.Transitions.GetTargetTypeForTransition<GetDeviceId>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<RegisterDevice>();
        }

        [Test]
        public void EnsureEncryptionKeys_OnSuccess_ShouldBeGetUser()
        {
            _sequence.Transitions.GetTargetTypeForTransition<EnsureEncryptionKeys>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<GetUser>();
        }

        [Test]
        public void EnsureEncryptionKeys_OnFail_ShouldBeFail()
        {
            _sequence.Transitions.GetTargetTypeForTransition<EnsureEncryptionKeys>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<Failed>();
        }

        [Test]
        public void GetDeviceId_Successful_ShouldBeEnsureEncryptionKeys()
        {
            _sequence.Transitions.GetTargetTypeForTransition<GetDeviceId>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<EnsureEncryptionKeys>();
        }

        [Test]
        public void RegisterDevice_Failed_ShouldBeFailed()
        {
            _sequence.Transitions.GetTargetTypeForTransition<RegisterDevice>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<Failed>();
        }

        [Test]
        public void RegisterDevice_Successful_ShouldBeGetUser()
        {
            _sequence.Transitions.GetTargetTypeForTransition<RegisterDevice>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<GetUser>();
        }

        [Test]
        public void VerifyNumberOfDevices_OnZero_ShouldBeFailed()
        {
            _sequence.Transitions.GetTargetTypeForTransition<VerifyNumberOfDevices>(NumberOfDevicesEnum.Zero)
                .Should()
                .Be<Failed>();
        }

        [Test]
        public void VerifyNumberOfDevices_OnOne_ShouldBeGetUser()
        {
            _sequence.Transitions.GetTargetTypeForTransition<VerifyNumberOfDevices>(NumberOfDevicesEnum.One)
                .Should()
                .Be<AddSampleClippings>();
        }

        [Test]
        public void VerifyNumberOfDevices_OnTwoAndThisOneIsNew_ShouldBeShowCongratulations()
        {
            _sequence.Transitions.GetTargetTypeForTransition<VerifyNumberOfDevices>(NumberOfDevicesEnum.TwoAndThisOneIsNew)
                .Should()
                .Be<ShowCreateClipping>();
        }

        [Test]
        public void VerifyNumberOfDevices_OnTwoOrMore_ShouldBeFinished()
        {
            _sequence.Transitions.GetTargetTypeForTransition<VerifyNumberOfDevices>(NumberOfDevicesEnum.TwoOrMore)
                .Should()
                .Be<Finished>();
        }

        [Test]
        public void AddSampleClippings_OnFail_ShouldBeFailed()
        {
            _sequence.Transitions.GetTargetTypeForTransition<AddSampleClippings>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<Failed>();
        }

        [Test]
        public void AddSampleClippings_OnSuccess_ShouldBeGetAndroidInstallLink()
        {
            _sequence.Transitions.GetTargetTypeForTransition<AddSampleClippings>(SimpleStepStateEnum.Successful)
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
                .Be<ShowAndroidInstallGuide>();
        }

        [Test]
        public void AndroidInstallGuide_OnFail_ShouldBeFailed()
        {
            _sequence.Transitions.GetTargetTypeForTransition<ShowAndroidInstallGuide>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<Failed>();
        }

        [Test]
        public void AndroidInstallGuide_OnSuccess_ShouldBeWaitForSecondDevice()
        {
            _sequence.Transitions.GetTargetTypeForTransition<ShowAndroidInstallGuide>(SimpleStepStateEnum.Successful)
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

        [Test]
        public void ShowCreateClipping_OnFailed_ShouldBeFailed()
        {
            _sequence.Transitions.GetTargetTypeForTransition<ShowCreateClipping>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<Failed>();
        }

        [Test]
        public void ShowCreateClipping_OnSuccess_ShouldBeWaitForLocalClipping()
        {
            _sequence.Transitions.GetTargetTypeForTransition<ShowCreateClipping>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<WaitForLocalClipping>();
        }

        [Test]
        public void WaitForLocalClipping_OnFailed_ShouldBeFailed()
        {
            _sequence.Transitions.GetTargetTypeForTransition<WaitForLocalClipping>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<Failed>();
        }

        [Test]
        public void WaitForLocalClipping_OnSuccess_ShouldBeFinished()
        {
            _sequence.Transitions.GetTargetTypeForTransition<WaitForLocalClipping>(SimpleStepStateEnum.Successful)
                .Should()
                .Be<Finished>();
        }
    }
}