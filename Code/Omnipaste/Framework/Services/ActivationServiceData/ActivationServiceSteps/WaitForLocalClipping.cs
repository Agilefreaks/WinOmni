namespace Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps
{
    using Clipboard.Handlers;

    public class WaitForLocalClipping : WaitForClippingBase
    {
        #region Constructors and Destructors

        public WaitForLocalClipping(ILocalClipboardHandler clippingSource)
            : base(clippingSource)
        {
        }

        #endregion
    }
}