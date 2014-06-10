using System.Threading.Tasks;
using Ninject;
using OmniApi.Resources;
using OmniCommon.Interfaces;
using Retrofit.Net;

namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniApi.Models;

    public class GetRemoteConfiguration : ActivationStepBase
    {
        private readonly IConfigurationService _configurationService;

        public IAuthorizationAPI AuthorizationAPI { get; set; }

        public const int MaxRetryCount = 5;

        private RetryInfo _payload;

        [Inject]
        public IKernel Kernel { get; set; }

        public override DependencyParameter Parameter { get; set; }

        private RetryInfo PayLoad
        {
            get
            {
                _payload = (Parameter.Value as RetryInfo) ?? new RetryInfo((string)Parameter.Value);
                return _payload;
            }
        }

        public GetRemoteConfiguration(IAuthorizationAPI authorizationApi, IConfigurationService configurationService)
        {
            _configurationService = configurationService;
            AuthorizationAPI = authorizationApi;
        }

        public override IExecuteResult Execute()
        {
            Task<IExecuteResult> executeAsync = ExecuteAsync();
            executeAsync.Wait();

            return executeAsync.Result;
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
                var activationModelTask = await AuthorizationAPI.Activate(PayLoad.Token, _configurationService.ClientId);
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
                var authenticator = new Authenticator
                                              {
                                                  AccessToken = activationModel.access_token,
                                                  RefreshToken = activationModel.refresh_token,
                                                  GrantType = activationModel.token_type
                                              };
                executeResult.State = GetRemoteConfigurationStepStateEnum.Successful;
                executeResult.Data = authenticator;
                
                Kernel.Bind<Authenticator>().ToConstant(authenticator);
            }
            else
            {
                executeResult.State = GetRemoteConfigurationStepStateEnum.Failed;
                executeResult.Data = activationModel.error_description;
            }
        }
    }
}