namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniCommon.Models;

    public interface IProxyConfigurationDetector
    {
        ProxyConfiguration Detect();
    }
}