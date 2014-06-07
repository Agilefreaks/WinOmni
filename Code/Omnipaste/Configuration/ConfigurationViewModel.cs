namespace Omnipaste.Configuration
{
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Framework;
    using Omnipaste.Services;

    public class ConfigurationViewModel : Screen, IConfigurationViewModel
    {
        #region Fields

        private readonly IActivationService _activationService;

        private readonly IEventAggregator _eventAggregator;

        #endregion

        #region Constructors and Destructors

        public ConfigurationViewModel(IActivationService activationService, IEventAggregator eventAggregator)
        {
            ApplicationWrapper = new ApplicationWrapper();
            _activationService = activationService;
            _eventAggregator = eventAggregator;
        }

        #endregion

        #region Public Properties

        public IApplicationWrapper ApplicationWrapper { get; set; }

        #endregion

        #region Public Methods and Operators

        public async Task Start()
        {
            await _activationService.Run();

            OnContinuationAction();
        }

        #endregion

        #region Methods

        private void OnContinuationAction()
        {
            if (_activationService.Success)
            {
                _eventAggregator.PublishOnCurrentThread(new ConfigurationCompletedMessage());
            }
            else
            {
                Execute.OnUIThread(() => ApplicationWrapper.ShutDown());
            }
        }

        #endregion
    }
}