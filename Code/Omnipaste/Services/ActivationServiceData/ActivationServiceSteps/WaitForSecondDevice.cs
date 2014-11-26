namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using OmniApi.Resources.v1;
    using OmniCommon.ExtensionMethods;

    public class WaitForSecondDevice : ActivationStepBase
    {
        #region Static Fields

        private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(2);

        #endregion

        #region Fields

        private readonly IDevices _devices;

        private readonly TimeSpan _checkInterval;

        #endregion

        #region Constructors and Destructors

        public WaitForSecondDevice(IDevices devices)
            : this(devices, CheckInterval)
        {
        }

        public WaitForSecondDevice(IDevices devices, TimeSpan checkInterval)
        {
            _checkInterval = checkInterval;
            _devices = devices;
        }

        #endregion

        #region Public Methods and Operators

        public override IObservable<IExecuteResult> Execute()
        {
            return
                Observable.Defer(() => _devices.GetAll().ReportErrors())
                    .RetryUntil(devices => devices.Count > 1, _checkInterval)
                    .Select(_ => new ExecuteResult(SimpleStepStateEnum.Successful))
                    .Take(1);
        }

        #endregion
    }
}