namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniApi.Cryptography;
    using OmniCommon.Interfaces;

    public class CreateEncryptionKeys : SynchronousStepBase
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        private readonly ICryptoService _cryptoService;

        #endregion

        #region Constructors and Destructors

        public CreateEncryptionKeys(ICryptoService cryptoService, IConfigurationService configurationService)
        {
            _cryptoService = cryptoService;
            _configurationService = configurationService;
        }

        #endregion

        #region Methods

        protected override IExecuteResult ExecuteSynchronously()
        {
            if (_configurationService.DeviceKeyPair == null
                || string.IsNullOrEmpty(_configurationService.DeviceKeyPair.Public)
                || string.IsNullOrEmpty(_configurationService.DeviceKeyPair.Private))
            {
                _configurationService.DeviceKeyPair = _cryptoService.GenerateKeyPair();
            }

            return new ExecuteResult { State = SimpleStepStateEnum.Successful };
        }

        #endregion
    }
}