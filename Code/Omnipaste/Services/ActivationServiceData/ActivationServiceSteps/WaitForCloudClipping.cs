namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;

    public class WaitForCloudClipping : ActivationStepBase
    {
        public override IObservable<IExecuteResult> Execute()
        {
            return Observable.Never<IExecuteResult>();
        }
    }
}