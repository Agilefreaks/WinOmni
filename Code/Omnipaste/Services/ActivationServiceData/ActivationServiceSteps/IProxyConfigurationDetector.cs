namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniCommon;

    public interface IProxyConfigurationDetector
    {
        ProxyConfiguration? Detect();
    }
}