namespace Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps
{
    using OmniCommon.Models;

    public interface IProxyConfigurationDetector
    {
        ProxyConfiguration Detect();
    }
}