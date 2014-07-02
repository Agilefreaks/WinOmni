namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    public class Failed : VoidStep
    {
        public override object State
        {
            get
            {
                return SimpleStepStateEnum.Failed;
            }
        }
    }
}