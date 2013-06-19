namespace OmniCommon.Services.ActivationServiceSteps
{
    public interface IActivationStep
    {
        IExecuteResult Execute();

        object GetId();
    }
}