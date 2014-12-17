namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public class InitDeviceInfos : ActivationStepBase
    {
        #region Fields

        private readonly IConfigurationService _configurationService;

        private readonly IDevices _devices;

        #endregion

        #region Constructors and Destructors

        public InitDeviceInfos(IDevices devices, IConfigurationService configurationService)
        {
            _devices = devices;
            _configurationService = configurationService;
        }

        #endregion

        #region Public Methods and Operators

        public override IObservable<IExecuteResult> Execute()
        {
            return _devices.GetAll().Select(CreateResult);
        }

        #endregion

        #region Methods

        private IExecuteResult CreateResult(List<Device> devices)
        {
            var currentDeviceIdentifier = _configurationService.DeviceIdentifier;
            _configurationService.DeviceInfos =
                devices.Where(device => device.Identifier != currentDeviceIdentifier)
                    .Select(
                        device =>
                        new DeviceInfo
                            {
                                Identifier = device.Identifier,
                                Name = device.Name,
                                Provider = device.Provider,
                                PublicKey = device.PublicKey
                            })
                    .ToList();

            return new ExecuteResult(SimpleStepStateEnum.Successful);
        }

        #endregion
    }
}