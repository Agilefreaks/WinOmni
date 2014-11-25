namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using OmniApi.Resources.v1;

    public class VerifyNumberOfDevices : ActivationStepBase
    {
        public VerifyNumberOfDevices(IDevices devices)
        {
            Devices = devices;
        }

        #region Public Properties

        public IDevices Devices { get; set; }

        #endregion

        #region Public Methods and Operators

        public override IObservable<IExecuteResult> Execute()
        {
            return Devices.GetAll().Select(
                deviceCount => deviceCount.Count <= 1
                    ? new ExecuteResult(SimpleStepStateEnum.Failed, null)
                    : new ExecuteResult(SimpleStepStateEnum.Successful, null));
        }

        #endregion
    }
}