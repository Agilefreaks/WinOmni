namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    public class RetryInfo
    {
        public string AuthorizationCode { get; set; }

        public int FailCount { get; private set; }

        public string Error { get; private set; }

        public RetryInfo(string authorizationCode, int retryCount = 0, string error = "")
        {
            AuthorizationCode = authorizationCode;
            FailCount = retryCount;
            Error = error;
        }
    }
}