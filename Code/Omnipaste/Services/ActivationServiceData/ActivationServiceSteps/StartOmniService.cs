namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using Omni;

    public class StartOmniService : ActivationStepBase
    {
        #region Fields

        private readonly IOmniService _omniService;

        #endregion

        #region Constructors and Destructors

        public StartOmniService(IOmniService omniService)
        {
            _omniService = omniService;
        }

        #endregion

        #region Methods

        protected override IObservable<IExecuteResult> InternalExecute()
        {
            return _omniService.Start().Select(d => new ExecuteResult(SimpleStepStateEnum.Successful));
        }

        #endregion
    }
}