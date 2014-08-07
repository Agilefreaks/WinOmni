namespace Omnipaste.Loading.ActivationFailed
{
    using System;
    using Caliburn.Micro;
    using Ninject;
    using OmniCommon.EventAggregatorMessages;
    using Omnipaste.Framework;

    public class ActivationFailedViewModel : Screen, IActivationFailedViewModel
    {
        #region Constructors and Destructors

        public ActivationFailedViewModel()
        {
            ApplicationService = new ApplicationService();
        }

        #endregion

        #region Public Properties

        public IApplicationService ApplicationService { get; set; }

        [Inject]
        public IEventAggregator EventAggregator { get; set; }

        public Exception Exception
        {
            set
            {
                ExceptionMessage = value.ToString();
            }
        }

        public string ExceptionMessage { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Exit()
        {
            ApplicationService.ShutDown();
        }

        public void Retry()
        {
            EventAggregator.PublishOnUIThread(new RetryMessage());
        }

        #endregion
    }
}