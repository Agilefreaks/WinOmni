namespace OmniCommon.Services.ActivationServiceData
{
    using System;
    using Ninject;
    using OmniCommon.Services.ActivationServiceData.ActivationServiceSteps;

    public class StepFactory : IStepFactory
    {
        [Inject]
        public IKernel Kernel { get; set; }

        public IActivationStep Create(Type type, object payload = null)
        {
            var activationStep = (IActivationStep)Kernel.Get(type);
            activationStep.Parameter = new DependencyParameter { Name = "payload", Value = payload };

            return activationStep;
        }
    }
}