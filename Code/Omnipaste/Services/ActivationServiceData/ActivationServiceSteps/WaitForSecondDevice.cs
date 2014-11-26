namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using Omni;
    using OmniApi.Resources.v1;
    using OmniCommon.Models;

    public class WaitForSecondDevice : ActivationStepBase
    {
        private readonly IOmniService _omniService;

        private readonly IDevices _devices;

        public WaitForSecondDevice(IOmniService omniService, IDevices devices)
        {
            _omniService = omniService;
            _devices = devices;
        }

        public override IObservable<IExecuteResult> Execute()
        {
            return
                _omniService.OmniMessageObservable.Where(message => message.Provider == OmniMessageTypeEnum.Device)
                    .Select(_ => _devices.GetAll())
                    .Switch()
                    .Where(devices => devices.Count > 1)
                    .Select(_ => new ExecuteResult(SimpleStepStateEnum.Successful))
                    .Take(1);
        }
    }
}