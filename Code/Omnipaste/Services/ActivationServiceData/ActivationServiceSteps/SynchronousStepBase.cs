namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    public abstract class SynchronousStepBase : ActivationStepBase
    {
        protected override IObservable<IExecuteResult> InternalExecute()
        {
            return Observable.Create<IExecuteResult>(
                observer =>
                    {
                        observer.OnNext(ExecuteSynchronously());
                        observer.OnCompleted();

                        return Disposable.Empty;
                    });
        }

        protected abstract IExecuteResult ExecuteSynchronously();
    }
}