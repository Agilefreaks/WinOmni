namespace Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps
{
    using Clipboard.Handlers;

    public class WaitForCloudClipping : WaitForClippingBase
    {
        #region Constructors and Destructors

        public WaitForCloudClipping(IOmniClipboardHandler omniClipboardHandler)
            : base(omniClipboardHandler)
        {
        }

        #endregion
    }
}