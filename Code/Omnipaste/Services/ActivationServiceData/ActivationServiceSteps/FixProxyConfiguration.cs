namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Collections.Generic;
    using System.Linq;
    using OmniCommon;
    using OmniCommon.Interfaces;

    public class FixProxyConfiguration : SynchronousStepBase
    {
        private readonly IConfigurationService _configurationService;

        private readonly IEnumerable<IProxyConfigurationDetector> _proxyConfigurationDetectors;

        public FixProxyConfiguration(IConfigurationService configurationService, IEnumerable<IProxyConfigurationDetector> proxyConfigurationDetectors)
        {
            _configurationService = configurationService;
            _proxyConfigurationDetectors = proxyConfigurationDetectors;
        }

        protected override IExecuteResult ExecuteSynchronously()
        {
            ExecuteResult executeResult;
            var existingConfiguration = _configurationService.ProxyConfiguration;
            var newConfiguration =
                _proxyConfigurationDetectors.Select(detector => detector.Detect())
                    .Where(configuration => configuration.HasValue)
                    .DefaultIfEmpty(ProxyConfiguration.Empty())
                    .FirstOrDefault(configuration => !Equals(existingConfiguration, configuration));
            if (newConfiguration.HasValue)
            {
                _configurationService.SaveProxyConfiguration(newConfiguration.Value);
                executeResult = new ExecuteResult(SimpleStepStateEnum.Successful);
            }
            else
            {
                executeResult = new ExecuteResult(SimpleStepStateEnum.Failed);
            }

            return executeResult;
        }
    }
}