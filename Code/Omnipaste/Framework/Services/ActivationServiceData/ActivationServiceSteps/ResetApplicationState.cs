namespace Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniCommon.Interfaces;

    public class ResetApplicationState : SynchronousStepBase
    {
        private readonly IConfigurationService _configurationService;

        public ResetApplicationState(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        protected override IExecuteResult ExecuteSynchronously()
        {
            _configurationService.ClearSettings();
            return new ExecuteResult(SimpleStepStateEnum.Successful);
        }
    }
}