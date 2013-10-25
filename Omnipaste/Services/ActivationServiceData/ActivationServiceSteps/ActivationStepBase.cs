namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    public abstract class ActivationStepBase : IActivationStep
    {
        public virtual DependencyParameter Parameter { get; set; }

        public abstract IExecuteResult Execute();

        public virtual object GetId()
        {
            return this.GetType();
        }
    }
}