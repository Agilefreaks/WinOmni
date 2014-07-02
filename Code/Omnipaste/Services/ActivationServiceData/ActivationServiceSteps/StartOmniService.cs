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

        #region Public Methods and Operators

        public override IObservable<IExecuteResult> Execute()
        {
            var failObserver = new[] { new ExecuteResult(SimpleStepStateEnum.Failed) }.ToObservable();
            return _omniService.Start().Select(d => new ExecuteResult(SimpleStepStateEnum.Successful)).Catch(failObserver);
        }

        #endregion
    }
}