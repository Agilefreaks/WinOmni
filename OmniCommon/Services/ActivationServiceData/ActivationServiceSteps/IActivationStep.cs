namespace OmniCommon.Services.ActivationServiceData.ActivationServiceSteps
{
    public interface IActivationStep
    {
        IExecuteResult Execute();

        object GetId();
    }
}