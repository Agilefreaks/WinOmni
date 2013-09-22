namespace Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData.ActivationServiceSteps
{
    public class RetryInfo
    {
        public string Token { get; set; }

        public int FailCount { get; private set; }

        public string Error { get; private set; }

        public RetryInfo(string token, int retryCount = 0, string error = "")
        {
            this.Token = token;
            this.FailCount = retryCount;
            this.Error = error;
        }
    }
}