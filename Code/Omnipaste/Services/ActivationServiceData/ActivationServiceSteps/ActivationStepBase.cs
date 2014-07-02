namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;

    public abstract class ActivationStepBase : IActivationStep
    {
        public virtual DependencyParameter Parameter { get; set; }

        public abstract IObservable<IExecuteResult> Execute();

        public virtual object GetId()
        {
            return GetType();
        }
    }
}