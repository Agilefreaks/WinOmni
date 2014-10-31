namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;

    public abstract class SynchronousStepBase : ActivationStepBase
    {
        public override IObservable<IExecuteResult> Execute()
        {
            return Observable.Start<IExecuteResult>(ExecuteSynchronously);
        }

        protected abstract IExecuteResult ExecuteSynchronously();
    }
}