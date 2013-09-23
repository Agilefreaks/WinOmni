namespace Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniCommon.DataProviders;
    using Omnipaste.OmniClipboard.Core.Api;
    using Omnipaste.OmniClipboard.Core.Api.Resources;
    using Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData;

    public class GetRemoteConfiguration : ActivationStepBase
    {
        public const int MaxRetryCount = 5;

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

        public IUsers Users { get; set; }

        public GetRemoteConfiguration(IUsers users)
        {
            Users = users;
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
                var activationData = Users.Activate(_payload.Token);
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