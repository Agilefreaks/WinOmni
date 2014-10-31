namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;

    public abstract class VoidStep : ActivationStepBase
    {
        public abstract SimpleStepStateEnum State { get; }

        public override IObservable<IExecuteResult> Execute()
        {
            return Observable.Empty<IExecuteResult>();
        }
    }
}