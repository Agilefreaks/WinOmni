namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    public class Failed : VoidStep
    {
        public override SimpleStepStateEnum State
        {
            get
            {
                return SimpleStepStateEnum.Failed;
            }
        }
    }
}