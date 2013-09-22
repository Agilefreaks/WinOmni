namespace Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniCommon.DataProviders;
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
                return this._parameter;
            }

            set
            {
                this._parameter = value;
                this._payload = (value.Value as RetryInfo) ?? new RetryInfo((string)value.Value);
            }
        }

        public GetRemoteConfiguration(IActivationDataProvider activationDataProvider)
        {
            this._activationDataProvider = activationDataProvider;
        }

        public override IExecuteResult Execute()
        {
            var executeResult = new ExecuteResult();
            if (string.IsNullOrEmpty(this._payload.Token))
            {
                executeResult.State = GetRemoteConfigurationStepStateEnum.Failed;
            }
            else
            {
                this.SetResultPropertiesBasedOnActivationData(executeResult);
            }

            return executeResult;
        }

        private void SetResultPropertiesBasedOnActivationData(IExecuteResult executeResult)
        {
            var activationData = this._activationDataProvider.GetActivationData(this._payload.Token);
            if (!string.IsNullOrEmpty(activationData.CommunicationError))
            {
                executeResult.Data = new RetryInfo(
                    this._payload.Token, this._payload.FailCount + 1, activationData.CommunicationError);
                executeResult.State = this._payload.FailCount < MaxRetryCount
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