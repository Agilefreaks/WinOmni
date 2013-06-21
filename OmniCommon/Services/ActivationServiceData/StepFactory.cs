namespace OmniCommon.Services.ActivationServiceData
{
    using System;
    using OmniCommon.Services.ActivationServiceData.ActivationServiceSteps;

    public class StepFactory : IStepFactory
    {
        private static readonly Type ActivationStepType = typeof(IActivationStep);

        private readonly IDependencyResolver _dependencyResolver;

        public StepFactory(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public IActivationStep Create(Type type, object payload = null)
        {
            var dependencyParameter = payload == null
                                          ? null
                                          : new DependencyParameter { Name = "payload", Value = payload };
            return ActivationStepType.IsAssignableFrom(type)
                       ? payload != null
                             ? _dependencyResolver.Get(type, dependencyParameter) as IActivationStep
                             : _dependencyResolver.Get(type) as IActivationStep
                       : null;
        }
    }
}