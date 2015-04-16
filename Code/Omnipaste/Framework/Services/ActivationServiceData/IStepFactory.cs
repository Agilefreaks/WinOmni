namespace Omnipaste.Framework.Services.ActivationServiceData
{
    using System;
    using Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps;

    public interface IStepFactory
    {
        IActivationStep Create(Type type, object payload = null);
    }
}