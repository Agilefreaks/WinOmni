namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    public class Finished : VoidStep
    {
        public override SimpleStepStateEnum State
        {
            get
            {
                return SimpleStepStateEnum.Successful;
            }
        }
    }
}