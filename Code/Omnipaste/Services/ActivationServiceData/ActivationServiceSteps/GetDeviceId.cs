namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniCommon.Interfaces;

    public class GetDeviceId : SynchronousStepBase
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        #endregion

        #region Constructors and Destructors

        public GetDeviceId(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        #endregion

        #region Methods

        protected override IExecuteResult ExecuteSynchronously()
        {
            var result = new ExecuteResult();
            var deviceIdentifier = _configurationService.DeviceIdentifier;
            if (string.IsNullOrWhiteSpace(deviceIdentifier))
            {
                result.State = SimpleStepStateEnum.Failed;
            }
            else
            {
                result.State = SimpleStepStateEnum.Successful;
                result.Data = deviceIdentifier;
            }

            return result;
        }

        #endregion
    }
}