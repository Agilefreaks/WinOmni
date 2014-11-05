namespace OmniDebug.DebugBar.IncomingClipping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Micro;
    using Clipboard.Models;
    using OmniCommon.Models;
    using OmniDebug.Services;

    public class IncomingClippingViewModel : PropertyChangedBase, IDebugBarPanel
    {
        #region Fields

        private readonly IClippingsWrapper _clippingsWrapper;

        private readonly IConnectionManagerWrapper _connectionManagerWrapper;

        private string _clippingContent;

        private Clipping.ClippingTypeEnum _clippingType;

        #endregion

        #region Constructors and Destructors

        public IncomingClippingViewModel(IConnectionManagerWrapper connectionManagerWrapper, IClippingsWrapper clippingsWrapper)
        {
            _connectionManagerWrapper = connectionManagerWrapper;
            _clippingsWrapper = clippingsWrapper;
        }

        #endregion

        #region Public Properties

        public string ClippingContent
        {
            get
            {
                return _clippingContent;
            }
            set
            {
                if (value == _clippingContent)
                {
                    return;
                }
                _clippingContent = value;
                NotifyOfPropertyChange();
            }
        }

        public Clipping.ClippingTypeEnum ClippingType
        {
            get
            {
                return _clippingType;
            }
            set
            {
                if (value == _clippingType)
                {
                    return;
                }
                _clippingType = value;
                NotifyOfPropertyChange();
            }
        }

        public IEnumerable<Clipping.ClippingTypeEnum> ClippingTypes
        {
            get
            {
                return Enum.GetValues(typeof(Clipping.ClippingTypeEnum)).Cast<Clipping.ClippingTypeEnum>();
            }
        }

        #endregion

        #region Public Methods and Operators

        public void SimulateIncomingClipping()
        {
            _clippingsWrapper.MockLast(new Clipping { Content = ClippingContent, Type = ClippingType });
            _connectionManagerWrapper.SimulateMessage(new OmniMessage(OmniMessageTypeEnum.Clipboard));
        }

        #endregion
    }
}