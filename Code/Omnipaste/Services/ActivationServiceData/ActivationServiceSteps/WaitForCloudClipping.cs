namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using Clipboard.Handlers;
    using OmniCommon.Helpers;

    public class WaitForCloudClipping : ActivationStepBase
    {
        private readonly IOmniClipboardHandler _omniClipboardHandler;

        public WaitForCloudClipping(IOmniClipboardHandler omniClipboardHandler)
        {
            _omniClipboardHandler = omniClipboardHandler;
        }

        public override IObservable<IExecuteResult> Execute()
        {
            return _omniClipboardHandler.Clippings.Select(_ => new ExecuteResult(SimpleStepStateEnum.Successful))
                .Take(1, SchedulerProvider.Default);
        }
    }
}