namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using System.Reactive.Linq;
    using OmniApi.Models;
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
                devices =>
                    {
                        NumberOfDevicesEnum numberOfDevices;
                        switch (devices.Count)
                        {
                            case 0:
                                numberOfDevices = NumberOfDevicesEnum.Zero;
                                break;
                            case 1:
                                numberOfDevices = NumberOfDevicesEnum.One;
                                break;
                            case 2:
                                numberOfDevices = Parameter != null && Parameter.Value is Device
                                                      ? NumberOfDevicesEnum.TwoAndThisOneIsNew
                                                      : NumberOfDevicesEnum.TwoOrMore;
                                break;
                            default:
                                numberOfDevices = NumberOfDevicesEnum.TwoOrMore;
                                break;
                        }

                        return new ExecuteResult(numberOfDevices);
                    });
        }

        #endregion
    }
}