using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Omnipaste.EventAggregatorMessages;
using Omnipaste.Framework;
using Omnipaste.Services;
using Omnipaste.Services.ActivationServiceData.ActivationServiceSteps;

namespace Omnipaste.Configuration
{
    public class ConfigurationViewModel : Screen, IConfigurationViewModel
    {
        private readonly IActivationService _activationService;

        private readonly IEventAggregator _eventAggregator;

        public IApplicationWrapper ApplicationWrapper { get; set; }

        public ConfigurationViewModel(IActivationService activationService, IEventAggregator eventAggregator)
        {
            ApplicationWrapper = new ApplicationWrapper();
            _activationService = activationService;
            _eventAggregator = eventAggregator;
        }

        public async Task Start()
        {
            await _activationService.Run();

            OnContinuationAction();
        }

        private void OnContinuationAction()
        {
            if (_activationService.CurrentStep == null || _activationService.CurrentStep.GetId().Equals(typeof(Failed)))
            {
                Execute.OnUIThread(() => ApplicationWrapper.ShutDown());
            }
            else
            {
                _eventAggregator.PublishOnBackgroundThread(new ConfigurationCompletedMessage());
            }
        }
    }
}