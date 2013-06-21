namespace OmniCommon.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniCommon.DataProviders;

    public class GetRemoteConfiguration : ActivationStepBase
    {
        private readonly IActivationDataProvider _activationDataProvider;
        private readonly string _token;

        public GetRemoteConfiguration(IActivationDataProvider activationDataProvider, object payload)
        {
            _activationDataProvider = activationDataProvider;
            _token = payload as string;
        }

        public override IExecuteResult Execute()
        {
            var executeResult = new ExecuteResult();
            if (string.IsNullOrEmpty(_token))
            {
                executeResult.State = GetConfigurationStepStateEnum.Failed;
            }
            else
            {
                SetResultPropertiesBasedOnActivationData(executeResult);
            }

            return executeResult;
        }

        private void SetResultPropertiesBasedOnActivationData(IExecuteResult executeResult)
        {
            var activationData = _activationDataProvider.GetActivationData(_token);
            if (!string.IsNullOrEmpty(activationData.CommunicationError))
            {
                executeResult.State = GetConfigurationStepStateEnum.CommunicationFailure;
                executeResult.Data = activationData.CommunicationError;
            }
            else if (!string.IsNullOrEmpty(activationData.Email))
            {
                executeResult.State = GetConfigurationStepStateEnum.Successful;
                executeResult.Data = activationData.Email;
            }
            else
            {
                executeResult.State = GetConfigurationStepStateEnum.Failed;
            }
        }
    }
}