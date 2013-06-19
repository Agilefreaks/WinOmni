namespace OmniCommon.Services.ActivationServiceSteps
{
    public abstract class ActivationStepBase : IActivationStep
    {
        public abstract IExecuteResult Execute();

        public virtual object GetId()
        {
            return GetType();
        }
    }
}