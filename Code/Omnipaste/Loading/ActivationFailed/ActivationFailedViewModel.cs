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
            ApplicationWrapper = new ApplicationWrapper();
        }

        #endregion

        #region Public Properties

        [Inject]
        public IEventAggregator EventAggregator { get; set; }

        public IApplicationWrapper ApplicationWrapper { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Exit()
        {
            ApplicationWrapper.ShutDown();
        }

        public void Retry()
        {
            EventAggregator.PublishOnUIThread(new RetryMessage());
        }

        #endregion
    }
}