namespace Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData
{
    using System;
    using Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData.ActivationServiceSteps;

    public interface IStepFactory
    {
        IActivationStep Create(Type type, object payload = null);
    }
}