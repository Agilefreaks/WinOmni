namespace Omnipaste.Services.ActivationServiceData
{
    using System.Collections.Generic;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;
    using Omnipaste.Services.ActivationServiceData.Transitions;

    public class ActivationSequenceProvider : IActivationSequenceProvider
    {
        #region Fields

        private readonly ActivationSequence _activationSequence;

        #endregion

        #region Constructors and Destructors

        public ActivationSequenceProvider()
        {
            var finalStepIds = new List<object> { typeof(Finished), typeof(Failed) };

            var initialStepId = typeof(VerifyConnectivity);

            var transitions =
                TransitionCollection.Builder()
                    .RegisterTransition<VerifyConnectivity, GetLocalActivationCode, FixProxyConfiguration>()
                    .RegisterTransition<FixProxyConfiguration, VerifyConnectivity, ShowConnectionTroubleshooter>()
                    .RegisterTransition<ShowConnectionTroubleshooter, VerifyConnectivity, VerifyConnectivity>()
                    .RegisterTransition<GetLocalActivationCode, GetDeviceId, GetActivationCodeFromArguments>()
                    .RegisterTransition<GetActivationCodeFromArguments, GetRemoteConfiguration, GetActivationCodeFromUser>()
                    .RegisterTransition<GetActivationCodeFromUser, GetRemoteConfiguration, GetActivationCodeFromUser>()
                    .RegisterTransition<GetRemoteConfiguration, SaveConfiguration, GetActivationCodeFromUser>()
                    .RegisterTransition<SaveConfiguration, GetDeviceId, Failed>()
                    .RegisterTransition<GetDeviceId, EnsureEncryptionKeys, RegisterDevice>()
                    .RegisterTransition<RegisterDevice, GetUser, Failed>()
                    .RegisterTransition<EnsureEncryptionKeys, GetUser, Failed>()
                    .RegisterTransition<GetUser, SaveUser, Failed>()
                    .RegisterTransition<SaveUser, InitDeviceInfos, Failed>()
                    .RegisterTransition<InitDeviceInfos, StartOmniService, Failed>()
                    .RegisterTransition<StartOmniService, VerifyNumberOfDevices, Failed>()
                    .RegisterTransition<VerifyNumberOfDevices, Failed, NumberOfDevicesEnum>(NumberOfDevicesEnum.Zero)
                    .RegisterTransition<VerifyNumberOfDevices, AddSampleClippings, NumberOfDevicesEnum>(NumberOfDevicesEnum.One)
                    .RegisterTransition<VerifyNumberOfDevices, ShowCreateClipping, NumberOfDevicesEnum>(NumberOfDevicesEnum.TwoAndThisOneIsNew)
                    .RegisterTransition<VerifyNumberOfDevices, Finished, NumberOfDevicesEnum>(NumberOfDevicesEnum.TwoOrMore)
                    .RegisterTransition<AddSampleClippings, GetAndroidInstallLink, Failed>()
                    .RegisterTransition<GetAndroidInstallLink, ShowAndroidInstallGuide, Failed>()
                    .RegisterTransition<ShowAndroidInstallGuide, WaitForSecondDevice, Failed>()
                    .RegisterTransition<WaitForSecondDevice, ShowCongratulations, Failed>()
                    .RegisterTransition<ShowCongratulations, WaitForCloudClipping, Failed>()
                    .RegisterTransition<WaitForCloudClipping, Finished, Failed>()
                    .RegisterTransition<ShowCreateClipping, WaitForLocalClipping, Failed>()
                    .RegisterTransition<WaitForLocalClipping, Finished, Failed>()
                    .Build();

            _activationSequence = new ActivationSequence
                                      {
                                          FinalStepIdIds = finalStepIds,
                                          InitialStepId = initialStepId,
                                          Transitions = transitions
                                      };
        }

        #endregion

        #region Public Methods and Operators

        public ActivationSequence Get()
        {
            return _activationSequence;
        }

        #endregion
    }
}