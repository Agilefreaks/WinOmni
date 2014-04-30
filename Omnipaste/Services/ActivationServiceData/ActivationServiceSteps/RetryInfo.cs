namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    public class RetryInfo
    {
        public string Token { get; set; }

        public int FailCount { get; private set; }

        public string Error { get; private set; }

        public RetryInfo(string token, int retryCount = 0, string error = "")
        {
            Token = token;
            FailCount = retryCount;
            Error = error;
        }
    }
}