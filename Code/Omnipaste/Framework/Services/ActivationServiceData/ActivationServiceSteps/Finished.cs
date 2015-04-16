namespace Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps
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