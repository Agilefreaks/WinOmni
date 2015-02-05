namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Net;
    using System.Reactive.Linq;
    using OmniApi.Resources.v1;
    using OmniApi.Support;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;

    public class VerifySettings : ActivationStepBase
    {
        private readonly IDevices _devices;

        private readonly IConfigurationService _configurationService;

        public VerifySettings(IDevices devices, IConfigurationService configurationService)
        {
            _devices = devices;
            _configurationService = configurationService;
        }

        public override IObservable<IExecuteResult> Execute()
        {
            return !string.IsNullOrEmpty(_configurationService.DeviceId)
                       ? VerifyDeviceExists()
                       : Observable.Return(new ExecuteResult(SimpleStepStateEnum.Successful), SchedulerProvider.Default);
        }

        private IObservable<IExecuteResult> VerifyDeviceExists()
        {
            return
                _devices.Get(_configurationService.DeviceId)
                    .Select(_ => new ExecuteResult(SimpleStepStateEnum.Successful))
                    .Catch<IExecuteResult, Exception>(
                        e =>
                        e.IsHttpError(HttpStatusCode.NotFound)
                            ? Observable.Return(new ExecuteResult(SimpleStepStateEnum.Failed))
                            : Observable.Throw<IExecuteResult>(e));
        }
    }
}