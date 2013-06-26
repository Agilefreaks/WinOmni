namespace OmniCommon.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniCommon.DataProviders;

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

        public GetRemoteConfiguration(IActivationDataProvider activationDataProvider)
        {
            _activationDataProvider = activationDataProvider;
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
                SetResultPropertiesBasedOnActivationData(executeResult);
            }

            return executeResult;
        }

        private void SetResultPropertiesBasedOnActivationData(IExecuteResult executeResult)
        {
            var activationData = _activationDataProvider.GetActivationData(_payload.Token);
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