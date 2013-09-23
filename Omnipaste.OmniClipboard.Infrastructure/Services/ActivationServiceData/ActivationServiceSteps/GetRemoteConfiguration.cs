namespace Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniCommon.DataProviders;
    using Omnipaste.OmniClipboard.Core.Api;
    using Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData;

    public class GetRemoteConfiguration : ActivationStepBase
    {
        public const int MaxRetryCount = 5;

        private readonly IActivationDataProvider _activationDataProvider;

        private RetryInfo _payload;

        private DependencyParameter _parameter;

        public override DependencyParameter Parameter
        {
            get
            {
                return _parameter;
            }

            set
            {
                _parameter = value;
                _payload = (value.Value as RetryInfo) ?? new RetryInfo((string)value.Value);
            }
        }

        public IOmniApi OmniApi  { get; set; }

        public GetRemoteConfiguration(IActivationDataProvider activationDataProvider, IOmniApi omniApi)
        {
            _activationDataProvider = activationDataProvider;
            OmniApi = omniApi;
        }

        public override IExecuteResult Execute()
        {
            var executeResult = new ExecuteResult();
            if (string.IsNullOrEmpty(_payload.Token))
            {
                executeResult.State = GetRemoteConfigurationStepStateEnum.Failed;
            }
            else
            {
                var activationData = OmniApi.Users.Activate(_payload.Token);
                SetResultPropertiesBasedOnActivationData(executeResult, activationData);
            }

            return executeResult;
        }

        private void SetResultPropertiesBasedOnActivationData(IExecuteResult executeResult, ActivationData activationData)
        {
            if (!string.IsNullOrEmpty(activationData.CommunicationError))
            {
                executeResult.Data = new RetryInfo(
                    _payload.Token, _payload.FailCount + 1, activationData.CommunicationError);
                executeResult.State = _payload.FailCount < MaxRetryCount
                                          ? GetRemoteConfigurationStepStateEnum.CommunicationFailure
                                          : GetRemoteConfigurationStepStateEnum.Failed;
            }
            else if (!string.IsNullOrEmpty(activationData.Email))
            {
                executeResult.State = GetRemoteConfigurationStepStateEnum.Successful;
                executeResult.Data = activationData.Email;
            }
            else
            {
                executeResult.State = GetRemoteConfigurationStepStateEnum.Failed;
            }
        }
    }
}