namespace OmniCommon.Services.ActivationServiceSteps
{
    using System;

    public class StepFactory : IStepFactory
    {
        private static readonly Type ActivationStepType = typeof(IActivationStep);

        public IActivationStep Create(Type type)
        {
            return ActivationStepType.IsAssignableFrom(type) ? Activator.CreateInstance(type) as IActivationStep : null;
        }
    }
}