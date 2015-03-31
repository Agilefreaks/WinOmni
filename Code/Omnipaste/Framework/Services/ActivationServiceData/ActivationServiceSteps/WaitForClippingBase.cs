namespace Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using Clipboard.Handlers;
    using OmniCommon.Helpers;

    public abstract class WaitForClippingBase : ActivationStepBase
    {
        #region Fields

        private readonly IClipboard _clippingSource;

        #endregion

        #region Constructors and Destructors

        protected WaitForClippingBase(IClipboard clippingSource)
        {
            _clippingSource = clippingSource;
        }

        #endregion

        #region Public Methods and Operators

        public override IObservable<IExecuteResult> Execute()
        {
            return _clippingSource.Clippings.Select(_ => new ExecuteResult(SimpleStepStateEnum.Successful))
                .Take(1, SchedulerProvider.Default);
        }

        #endregion
    }
}