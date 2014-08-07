namespace Omnipaste.Loading.ActivationFailed
{
    using System;
    using Caliburn.Micro;
    using Ninject;
    using Omnipaste.Framework;

    public interface IActivationFailedViewModel : IScreen
    {
        #region Public Properties

        IApplicationService ApplicationService { get; set; }

        [Inject]
        IEventAggregator EventAggregator { get; set; }

        Exception Exception { set; }

        #endregion

        #region Public Methods and Operators

        void Exit();

        void Retry();

        #endregion
    }
}