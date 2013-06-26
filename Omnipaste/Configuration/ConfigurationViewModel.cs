namespace Omnipaste.Configuration
{
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using OmniCommon.Interfaces;
    using OmniCommon.Services.ActivationServiceData.ActivationServiceSteps;
    using Omnipaste.Framework;

    public class ConfigurationViewModel : Screen, IConfigurationViewModel
    {
        private readonly IActivationService _activationService;

        public IApplicationWrapper ApplicationWrapper { get; set; }

        public ConfigurationViewModel(IActivationService activationService)
        {
            ApplicationWrapper = new ApplicationWrapper();
            _activationService = activationService;
        }

        public void Start()
        {
            Task.Factory.StartNew(() => _activationService.Run()).ContinueWith(OnContinuationAction);
        }

        private void OnContinuationAction(Task result)
        {
            if (this._activationService.CurrentStep == null || this._activationService.CurrentStep.GetId().Equals(typeof(Failed)))
            {
                Execute.OnUIThread(() => ApplicationWrapper.ShutDown());
            }
            else
            {
                // do nothing
            }
        }
    }
}