namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using Omni;
    using OmniCommon.Helpers;
    using OmniCommon.Models;

    public class WaitForCloudClipping : ActivationStepBase
    {
        private readonly IOmniService _omniService;

        public WaitForCloudClipping(IOmniService omniService)
        {
            _omniService = omniService;
        }

        public override IObservable<IExecuteResult> Execute()
        {
            return
                _omniService.OmniMessageObservable.Where(
                    message => message.Provider == OmniMessageTypeEnum.Clipboard)
                    .Select(_ => new ExecuteResult(SimpleStepStateEnum.Successful))
                    .Take(1, SchedulerProvider.Default);
        }
    }
}