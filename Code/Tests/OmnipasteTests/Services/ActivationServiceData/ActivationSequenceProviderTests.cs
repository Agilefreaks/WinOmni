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
        public void VerifyNumberOfDevices_OnFailed_ShouldBeAndroidInstallGuide()
        {
            _sequence.Transitions.GetTargetTypeForTransition<VerifyNumberOfDevices>(SimpleStepStateEnum.Failed)
                .Should()
                .Be<AndroidInstallGuide>();
        }

        [Test]
        public void VerifyNumberOfDevices_OnSuccess_ShouldBeFinished()
        {
            _sequence.Transitions.GetTargetTypeForTransition<VerifyNumberOfDevices>(SimpleStepStateEnum.Successful)
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