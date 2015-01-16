namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using OmniApi.Cryptography;
    using OmniApi.Resources.v1;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using OmniCommon.Settings;

    public class RegisterDevice : ActivationStepBase
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        private readonly ICryptoService _cryptoService;

        private readonly IConfigurationContainer _configurationContainer;

        private readonly IDevices _devices;

        #endregion

        #region Constructors and Destructors

        public RegisterDevice(
            IDevices devices,
            IConfigurationService configurationService,
            ICryptoService cryptoService,
            IConfigurationContainer configurationContainer)
        {
            _devices = devices;
            _configurationService = configurationService;
            _cryptoService = cryptoService;
            _configurationContainer = configurationContainer;
        }

        #endregion

        #region Public Methods and Operators

        public override IObservable<IExecuteResult> Execute()
        {
            SimpleLogger.Log("Registering Device");
            var deviceIdentifier = _configurationService.DeviceIdentifier;
            return !string.IsNullOrWhiteSpace(deviceIdentifier)
                                     ? UpdateDeviceIdForRegisteredDevice(deviceIdentifier)
                                     : RegisterAsNewDevice();
        }

        //ToDo: remove once the migration from using DeviceIdentifiers is complete.
        private IObservable<IExecuteResult> UpdateDeviceIdForRegisteredDevice(string deviceIdentifier)
        {
            return
                _devices.GetAll()
                    .Select(deviceList => deviceList.Where(device => device.Name == deviceIdentifier).ToList())
                    .Do(
                        matchingDevices =>
                        _configurationContainer.SetValue(ConfigurationProperties.DeviceId, matchingDevices.First().Id))
                    .Select(_ => new ExecuteResult(SimpleStepStateEnum.Successful));
        }

        private IObservable<IExecuteResult> RegisterAsNewDevice()
        {
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