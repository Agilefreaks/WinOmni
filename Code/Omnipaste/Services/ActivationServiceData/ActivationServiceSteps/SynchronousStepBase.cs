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
                        try
                        {
                            observer.OnNext(ExecuteSynchronously());
                        }
                        catch (Exception exception)
                        {
                            observer.OnError(exception);
                        }

                        observer.OnCompleted();

                        return Disposable.Empty;
                    });
        }

        protected abstract IExecuteResult ExecuteSynchronously();
    }
}