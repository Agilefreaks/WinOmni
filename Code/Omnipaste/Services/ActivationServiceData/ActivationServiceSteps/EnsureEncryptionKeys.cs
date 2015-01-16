namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using OmniApi.Cryptography;
    using OmniApi.Resources.v1;
    using OmniCommon.Interfaces;

    public class EnsureEncryptionKeys : ActivationStepBase
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        private readonly ICryptoService _cryptoService;

        private readonly IDevices _devices;

        #endregion

        #region Constructors and Destructors

        public EnsureEncryptionKeys(
            ICryptoService cryptoService,
            IConfigurationService configurationService,
            IDevices devices)
        {
            _cryptoService = cryptoService;
            _configurationService = configurationService;
            _devices = devices;
        }

        #endregion

        #region Public Methods and Operators

        public override IObservable<IExecuteResult> Execute()
        {
            IObservable<IExecuteResult> result;

            var currentEncryptionKey = _configurationService.DeviceKeyPair;
            if (currentEncryptionKey == null || string.IsNullOrEmpty(currentEncryptionKey.Public))
            {
                currentEncryptionKey = _cryptoService.GenerateKeyPair();

                result =
                    _devices.Update(_configurationService.DeviceId, new { PublicKey = currentEncryptionKey.Public })
                        .Do(_ => _configurationService.DeviceKeyPair = currentEncryptionKey)
                        .Select(_ => new ExecuteResult { State = SimpleStepStateEnum.Successful });
            }
            else
            {
                result = Observable.Return(new ExecuteResult { State = SimpleStepStateEnum.Successful });
            }

            return result;
        }

        #endregion
    }
}