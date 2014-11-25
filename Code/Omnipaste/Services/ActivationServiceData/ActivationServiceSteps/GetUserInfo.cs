namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniApi.Models;

    public class GetUserInfo : SynchronousStepBase
    {
        protected override IExecuteResult ExecuteSynchronously()
        {
            return new ExecuteResult
                       {
                           State = SimpleStepStateEnum.Successful,
                           Data = new UserInfo { Email = "someEmail@test.com" }
                       };
        }
    }
}