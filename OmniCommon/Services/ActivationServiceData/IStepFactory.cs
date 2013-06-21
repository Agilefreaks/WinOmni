namespace OmniCommon.Services.ActivationServiceData
{
    using System;
    using OmniCommon.Services.ActivationServiceData.ActivationServiceSteps;

    public interface IStepFactory
    {
        IActivationStep Create(Type type, object payload = null);
    }
}