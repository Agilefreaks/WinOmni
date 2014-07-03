namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    public interface IExecuteResult
    {
        SimpleStepStateEnum State { get; set; }

        object Data { get; set; }
    }
}