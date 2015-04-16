namespace Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using OmniCommon.Helpers;

    public abstract class SynchronousStepBase : ActivationStepBase
    {
        public override IObservable<IExecuteResult> Execute()
        {
            return Observable.Start<IExecuteResult>(ExecuteSynchronously, SchedulerProvider.Default);
        }

        protected abstract IExecuteResult ExecuteSynchronously();
    }
}