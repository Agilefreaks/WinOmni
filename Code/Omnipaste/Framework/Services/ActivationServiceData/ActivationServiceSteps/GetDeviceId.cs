namespace Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps
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
            var deviceId = _configurationService.DeviceId;
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                result.State = SimpleStepStateEnum.Failed;
            }
            else
            {
                result.State = SimpleStepStateEnum.Successful;
                result.Data = deviceId;
            }

            return result;
        }

        #endregion
    }
}