namespace Omnipaste.Services.ActivationServiceData
{
    using System;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    public interface IStepFactory
    {
        IActivationStep Create(Type type, object payload = null);
    }
}