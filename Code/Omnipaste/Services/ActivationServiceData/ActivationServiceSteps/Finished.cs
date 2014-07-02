namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    public class Finished : VoidStep
    {
        public override object State
        {
            get
            {
                return SimpleStepStateEnum.Successful;
            }
        }
    }
}