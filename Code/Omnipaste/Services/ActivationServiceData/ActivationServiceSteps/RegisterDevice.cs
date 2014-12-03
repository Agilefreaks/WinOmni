namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using OmniApi.Resources.v1;
    using OmniCommon;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;

    public class RegisterDevice : ActivationStepBase
    {
        private readonly IDevices _devices;

        private readonly IConfigurationService _configurationService;

        public RegisterDevice(IDevices devices, IConfigurationService configurationService)
        {
            _devices = devices;
            _configurationService = configurationService;
        }

        public override IObservable<IExecuteResult> Execute()
        {
            SimpleLogger.Log("Registering Device");
            return
                _devices.Create(Guid.NewGuid().ToString(), _configurationService.MachineName)
                    .Select(
                        device =>
                        Observable.Start(
                            () => _configurationService.DeviceIdentifier = device.Identifier,
                            SchedulerProvider.Default))
                    .Switch()
                    .Select(device => new ExecuteResult(SimpleStepStateEnum.Successful, device));
        }
    }
}