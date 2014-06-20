namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Threading.Tasks;
    using Ninject;
    using OmniApi.Models;
    using OmniApi.Resources;
    using OmniCommon.Interfaces;
    using Retrofit.Net;

    public class GetRemoteConfiguration : ActivationStepBase
    {
        #region Constants

        public const int MaxRetryCount = 5;

        #endregion

        #region Fields

        private readonly IConfigurationService _configurationService;

        private RetryInfo _payload;

        #endregion

        #region Constructors and Destructors

        public GetRemoteConfiguration(IAuthorizationAPI authorizationApi, IConfigurationService configurationService)
        {
            _configurationService = configurationService;
            AuthorizationApi = authorizationApi;
        }

        #endregion

        #region Public Properties

        public IAuthorizationAPI AuthorizationApi { get; set; }

        [Inject]
        public IKernel Kernel { get; set; }

        public override DependencyParameter Parameter { get; set; }

        #endregion

        #region Properties

        private RetryInfo PayLoad
        {
            get
            {
                _payload = (Parameter.Value as RetryInfo) ?? new RetryInfo((string)Parameter.Value);
                return _payload;
            }
        }

        #endregion

        #region Public Methods and Operators

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
                var activationModelTask = await AuthorizationApi.Activate(PayLoad.Token, _configurationService.ClientId);
                SetResultPropertiesBasedOnActivationData(executeResult, activationModelTask.Data);
            }

            return executeResult;
        }

        #endregion

        #region Methods

        private void SetResultPropertiesBasedOnActivationData(
            IExecuteResult executeResult,
            ActivationModel activationModel)
        {
            if (activationModel == null)
            {
                executeResult.Data = new RetryInfo(_payload.Token, _payload.FailCount + 1);
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
                Kernel.Unbind<Authenticator>();
                Kernel.Bind<Authenticator>().ToConstant(authenticator);
            }
            else
            {
                executeResult.State = GetRemoteConfigurationStepStateEnum.Failed;
                executeResult.Data = activationModel.error_description;
            }
        }

        #endregion
    }
}