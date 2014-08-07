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

        protected override IObservable<IExecuteResult> InternalExecute()
        {
            return Devices.GetAll().Select(
                deviceCount => deviceCount.Count <= 1 
                    ? new ExecuteResult(SimpleStepStateEnum.Successful, null) 
                    : new ExecuteResult(SimpleStepStateEnum.Failed, null));
        }

        #endregion
    }
}