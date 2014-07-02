namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;

    public interface IActivationStep
    {
        DependencyParameter Parameter { get; set; }

        IObservable<IExecuteResult> Execute();

        object GetId();
    }
}