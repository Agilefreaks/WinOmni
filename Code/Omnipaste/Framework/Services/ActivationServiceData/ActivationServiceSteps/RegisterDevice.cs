namespace Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using OmniApi.Cryptography;
    using OmniApi.Resources.v1;
    using OmniCommon;
    using OmniCommon.Interfaces;

    public class RegisterDevice : ActivationStepBase
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        private readonly ICryptoService _cryptoService;

        private readonly IDevices _devices;

        #endregion

        #region Constructors and Destructors

        public RegisterDevice(
            IDevices devices,
            IConfigurationService configurationService,
            ICryptoService cryptoService)
        {
            _devices = devices;
            _configurationService = configurationService;
            _cryptoService = cryptoService;
        }

        #endregion

        #region Public Methods and Operators

        public override IObservable<IExecuteResult> Execute()
        {
            SimpleLogger.Log("Registering Device");
            var keyPair = _cryptoService.GenerateKeyPair();

            return _devices.Create(_configurationService.MachineName, keyPair.Public).Do(
                device =>
                    {
                        _configurationService.DeviceKeyPair = keyPair;
                        _configurationService.DeviceId = device.Id;
                    }).Select(_ => new ExecuteResult(SimpleStepStateEnum.Successful));
        }

        #endregion
    }
}