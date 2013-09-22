extern alias TasksNET35;
using Task = TasksNET35::System.Threading.Tasks.Task;

namespace Omnipaste.Configuration
{
    using Caliburn.Micro;
    using OmniCommon.Interfaces;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Framework;
    using Omnipaste.OmniClipboard.Infrastructure.Services;
    using Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData.ActivationServiceSteps;

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

        public void Start()
        {
            Task.Factory.StartNew(() => _activationService.Run()).ContinueWith(OnContinuationAction);
        }

        private void OnContinuationAction(Task result)
        {
            if (_activationService.CurrentStep == null || _activationService.CurrentStep.GetId().Equals(typeof(Failed)))
            {
                Execute.OnUIThread(() => ApplicationWrapper.ShutDown());
            }
            else
            {
                _eventAggregator.Publish(new ConfigurationCompletedMessage());
            }
        }
    }
}