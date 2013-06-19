namespace OmniCommon.Services.ActivationServiceSteps
{
    using System;

    public interface IStepFactory
    {
        IActivationStep Create(Type type);
    }
}