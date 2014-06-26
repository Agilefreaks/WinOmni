namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using Ninject;
    using OmniCommon.Interfaces;

    public class LoadLocalConfiguration : ActivationStepBase
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        #endregion

        #region Constructors and Destructors

        public LoadLocalConfiguration(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        #endregion

        #region Public Properties

        [Inject]
        public IKernel Kernel { get; set; }

        #endregion

        #region Public Methods and Operators

        public override IExecuteResult Execute()
        {
            var result = new ExecuteResult();

            var accessToken = _configurationService.AccessToken;
            
            if (string.IsNullOrEmpty(accessToken))
            {
                result.State = SimpleStepStateEnum.Failed;
            }
            else
            {
                result.Data = accessToken;
                result.State = SimpleStepStateEnum.Successful;
            }

            return result;
        }

        #endregion
    }
}