namespace Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData.ActivationServiceSteps
{
    using Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData;

    public abstract class ActivationStepBase : IActivationStep
    {
        public virtual DependencyParameter Parameter { get; set; }

        public abstract IExecuteResult Execute();

        public virtual object GetId()
        {
            return GetType();
        }
    }
}