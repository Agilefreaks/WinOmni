namespace OmniDebug.DebugBar.IncomingClipping
{
    using Clipboard.Models;
    using OmniCommon.Models;
    using OmniDebug.Services;

    public class IncomingClippingViewModel : IDebugBarPanel
    {
        #region Fields

        private readonly IClippingsWrapper _clippingsWrapper;

        private readonly IOmniServiceWrapper _omniServiceWrapper;

        #endregion

        #region Constructors and Destructors

        public IncomingClippingViewModel(IOmniServiceWrapper omniServiceWrapper, IClippingsWrapper clippingsWrapper)
        {
            _omniServiceWrapper = omniServiceWrapper;
            _clippingsWrapper = clippingsWrapper;
        }

        #endregion

        #region Public Methods and Operators

        public void SimulateIncomingClipping()
        {
            _clippingsWrapper.MockLast(new Clipping { Content = "test", Type = Clipping.ClippingTypeEnum.Unknown });
            _omniServiceWrapper.SimulateMessage(new OmniMessage(OmniMessageTypeEnum.Clipboard));
        }

        #endregion
    }
}