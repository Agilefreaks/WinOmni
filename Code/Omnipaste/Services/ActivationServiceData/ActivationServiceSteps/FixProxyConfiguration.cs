﻿namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System.Collections.Generic;
    using System.Linq;
    using OmniCommon;
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
            var newConfiguration =
                _proxyConfigurationDetectors.Select(detector => detector.Detect())
                    .Where(configuration => configuration.HasValue)
                    .DefaultIfEmpty(ProxyConfiguration.Empty())
                    .Where(configuration => !Equals(existingConfiguration, configuration))
                    .FirstOrDefault(configuration => _networkService.CanPingHome(configuration));
            
            if (newConfiguration.HasValue)
            {
                SimpleLogger.Log("Detected new proxy configuration");
                SimpleLogger.Log("New proxy type: " + newConfiguration.Value.Type);
                SimpleLogger.Log("New proxy address: " + newConfiguration.Value.Address);
                SimpleLogger.Log("New proxy port: " + newConfiguration.Value.Port);
                _configurationService.SaveProxyConfiguration(newConfiguration.Value);
                executeResult = new ExecuteResult(SimpleStepStateEnum.Successful);
            }
            else
            {
                SimpleLogger.Log("Could not find any new proxy configuration");
                executeResult = new ExecuteResult(SimpleStepStateEnum.Failed);
            }

            return executeResult;
        }
    }
}