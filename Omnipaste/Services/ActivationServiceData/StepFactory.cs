namespace Omnipaste.Services.ActivationServiceData
{
    using System;
    using Ninject;
    using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

    public class StepFactory : IStepFactory
    {
        [Inject]
        public IKernel Kernel { get; set; }

        public IActivationStep Create(Type type, object payload = null)
        {
            var activationStep = (IActivationStep)this.Kernel.Get(type);
            activationStep.Parameter = new DependencyParameter { Name = "payload", Value = payload };

            return activationStep;
        }
    }
}