using System.Threading.Tasks;
using OmniApi.Resources;
using OmniCommon.Interfaces;

namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniApi.Models;

    public class GetRemoteConfiguration : ActivationStepBase
    {
        private readonly IConfigurationService _configurationService;

        public IAuthorizationAPI _authorizationAPI { get; set; }

        public const int MaxRetryCount = 5;

        private RetryInfo _payload;

        public override DependencyParameter Parameter { get; set; }

        private RetryInfo PayLoad
        {
            get
            {
                _payload = (Parameter.Value as RetryInfo) ?? new RetryInfo((string)Parameter.Value);
                return _payload;
            }
        }

        public GetRemoteConfiguration(IAuthorizationAPI authorizationAPI, IConfigurationService configurationService)
        {
            _configurationService = configurationService;
            _authorizationAPI = authorizationAPI;
        }

        public override IExecuteResult Execute()
        {
            return new ExecuteResult();
        }

        public override async Task<IExecuteResult> ExecuteAsync()
        {
            var executeResult = new ExecuteResult();

            if (string.IsNullOrEmpty(PayLoad.Token))
            {
                executeResult.State = GetRemoteConfigurationStepStateEnum.Failed;
            }
            else
            {
                var activationModelTask = await _authorizationAPI.Activate(PayLoad.Token, _configurationService.GetClientId());
                SetResultPropertiesBasedOnActivationData(executeResult, activationModelTask.Data);
            }

            return executeResult;
        }

        private void SetResultPropertiesBasedOnActivationData(IExecuteResult executeResult, ActivationModel activationModel)
        {
            if (activationModel == null)
            {
                executeResult.Data = new RetryInfo(
                    _payload.Token, _payload.FailCount + 1);
                executeResult.State = _payload.FailCount < MaxRetryCount
                                          ? GetRemoteConfigurationStepStateEnum.CommunicationFailure
                                          : GetRemoteConfigurationStepStateEnum.Failed;
            }
            else if (!string.IsNullOrEmpty(activationModel.access_token))
            {
                executeResult.State = GetRemoteConfigurationStepStateEnum.Successful;
                executeResult.Data = activationModel.access_token;
            }
            else
            {
                executeResult.State = GetRemoteConfigurationStepStateEnum.Failed;
            }
        }
    }
}