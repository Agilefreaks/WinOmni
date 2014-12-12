namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Collections.Generic;
    using System.Linq;
    using OmniCommon;
    using OmniCommon.Models;
    using OmniCommon.Interfaces;

    public class FixProxyConfiguration : SynchronousStepBase
    {
        private readonly IConfigurationService _configurationService;

        private readonly IEnumerable<IProxyConfigurationDetector> _proxyConfigurationDetectors;

        private readonly INetworkService _networkService;

        public FixProxyConfiguration(
            IConfigurationService configurationService,
            IEnumerable<IProxyConfigurationDetector> proxyConfigurationDetectors,
            INetworkService networkService)
        {
            _configurationService = configurationService;
            _proxyConfigurationDetectors = proxyConfigurationDetectors;
            _networkService = networkService;
        }

        protected override IExecuteResult ExecuteSynchronously()
        {
            ExecuteResult executeResult;
            var existingConfiguration = _configurationService.ProxyConfiguration;
            SimpleLogger.Log("Trying to detect proxy configuration");
            var newConfigurations =
                _proxyConfigurationDetectors.Select(detector => detector.Detect())
                    .Where(configuration => configuration != null)
                    .DefaultIfEmpty(ProxyConfiguration.Empty())
                    .Where(configuration => !Equals(existingConfiguration, configuration))
                    .ToList();
            var workingConfiguration =
                newConfigurations.FirstOrDefault(configuration => _networkService.CanPingHome(configuration));
            var newConfiguration = workingConfiguration ?? newConfigurations.FirstOrDefault();

            if (newConfiguration != null)
            {
                SimpleLogger.Log("Detected new proxy configuration");
                SimpleLogger.Log("New proxy type: " + newConfiguration.Type);
                SimpleLogger.Log("New proxy address: " + newConfiguration.Address);
                SimpleLogger.Log("New proxy port: " + newConfiguration.Port);
                _configurationService.ProxyConfiguration = newConfiguration;
            }

            if(workingConfiguration != null)
            {
                SimpleLogger.Log("Found working proxy configuration");
                executeResult = new ExecuteResult(SimpleStepStateEnum.Successful);
            }
            else
            {
                SimpleLogger.Log("Could not find working proxy configuration");
                executeResult = new ExecuteResult(SimpleStepStateEnum.Failed);
            }

            return executeResult;
        }
    }
}