namespace Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData.ActivationServiceSteps
{
    public class VoidStep : ActivationStepBase
    {
        public override IExecuteResult Execute()
        {
            return new ExecuteResult { State = SingleStateEnum.Successful };
        }
    }
}