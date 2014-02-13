using System.Threading.Tasks;
using OmniApi.Resources;
using RestSharp;

namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniApi.Models;

    public class GetRemoteConfiguration : ActivationStepBase
    {
        public IActivationTokenAPI ActivationTokenAPI { get; set; }

        public const int MaxRetryCount = 5;

        private RetryInfo _payload;

        public override DependencyParameter Parameter { get; set; }

        public IActivationTokenAPI ActivationTokens { get; set; }

        private RetryInfo PayLoad
        {
            get
            {
                _payload = (Parameter.Value as RetryInfo) ?? new RetryInfo((string)Parameter.Value);
                return _payload;
            }
        }

        public GetRemoteConfiguration(IActivationTokenAPI activationTokenAPI)
        {
            ActivationTokenAPI = activationTokenAPI;
        }

        public override IExecuteResult Execute()
        {
            var executeResult = new ExecuteResult();
            if (string.IsNullOrEmpty(PayLoad.Token))
            {
                executeResult.State = GetRemoteConfigurationStepStateEnum.Failed;
            }
            else
            {
                var activationModelTask = ActivationTokens.Activate(PayLoad.Token);
                activationModelTask.Wait();
                SetResultPropertiesBasedOnActivationData(executeResult, activationModelTask.Result.Data);
            }

            return executeResult;
        }

        private void SetResultPropertiesBasedOnActivationData(IExecuteResult executeResult, ActivationModel activationModel)
        {
            if (!string.IsNullOrEmpty(activationModel.CommunicationError))
            {
                executeResult.Data = new RetryInfo(
                    _payload.Token, _payload.FailCount + 1, activationModel.CommunicationError);
                executeResult.State = _payload.FailCount < MaxRetryCount
                                          ? GetRemoteConfigurationStepStateEnum.CommunicationFailure
                                          : GetRemoteConfigurationStepStateEnum.Failed;
            }
            else if (!string.IsNullOrEmpty(activationModel.Email))
            {
                executeResult.State = GetRemoteConfigurationStepStateEnum.Successful;
                executeResult.Data = activationModel.Email;
            }
            else
            {
                executeResult.State = GetRemoteConfigurationStepStateEnum.Failed;
            }
        }
    }
}