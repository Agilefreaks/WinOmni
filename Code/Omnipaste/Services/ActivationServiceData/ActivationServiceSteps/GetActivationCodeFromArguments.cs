namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniCommon.DataProviders;

    public class GetActivationCodeFromArguments : SynchronousStepBase
    {
        private readonly IArgumentsDataProvider _dataProvider;

        public GetActivationCodeFromArguments(IArgumentsDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        protected override IExecuteResult ExecuteSynchronously()
        {
            var result = new ExecuteResult { State = SimpleStepStateEnum.Failed };
            var token = _dataProvider.AuthorizationKey;
            if (!string.IsNullOrEmpty(token))
            {
                result.State = SimpleStepStateEnum.Successful;
                result.Data = token;
            }

            return result;
        }
    }
}