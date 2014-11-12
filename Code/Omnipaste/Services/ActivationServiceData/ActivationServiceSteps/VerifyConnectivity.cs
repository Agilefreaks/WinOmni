namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    public class VerifyConnectivity : SynchronousStepBase
    {
        private readonly INetworkService _networkService;

        public VerifyConnectivity(INetworkService  networkService)
        {
            _networkService = networkService;
        }

        protected override IExecuteResult ExecuteSynchronously()
        {
            var executeResult =
                new ExecuteResult(
                    _networkService.CanPingHome() ? SimpleStepStateEnum.Successful : SimpleStepStateEnum.Failed);

            return executeResult;
        }
    }
}