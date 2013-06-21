namespace OmniCommon.Services.ActivationServiceData.ActivationServiceSteps
{
    public class RetryInfo
    {
        public RetryInfo(string token, int retryCount = 0, string error = "")
        {
            Token = token;
            FailCount = retryCount;
            Error = error;
        }

        public string Token { get; private set; }

        public int FailCount { get; private set; }

        public string Error { get; private set; }
    }
}