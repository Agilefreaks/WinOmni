namespace OmniDebug.DebugBar.IncomingClipping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Micro;
    using Clipboard.Dto;
    using OmniCommon.Models;
    using OmniDebug.Services;

    public class IncomingClippingViewModel : PropertyChangedBase, IDebugBarPanel
    {
        #region Fields

        private readonly IClippingsWrapper _clippingsWrapper;

        private readonly IOmniServiceWrapper _omniServiceWrapper;

        private string _clippingContent;

        private ClippingDto.ClippingTypeEnum _clippingType;

        #endregion

        #region Constructors and Destructors

        public IncomingClippingViewModel(IOmniServiceWrapper omniServiceWrapper, IClippingsWrapper clippingsWrapper)
        {
            _omniServiceWrapper = omniServiceWrapper;
            _clippingsWrapper = clippingsWrapper;
            ClippingContent = "some content";
            ClippingType = ClippingDto.ClippingTypeEnum.Unknown;
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

        public ClippingDto.ClippingTypeEnum ClippingType
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

        public IEnumerable<ClippingDto.ClippingTypeEnum> ClippingTypes
        {
            get
            {
                return Enum.GetValues(typeof(ClippingDto.ClippingTypeEnum)).Cast<ClippingDto.ClippingTypeEnum>();
            }
        }

        #endregion

        #region Public Methods and Operators

        public void SimulateIncomingClipping()
        {
            var clippingId = Guid.NewGuid().ToString();
            _clippingsWrapper.MockGet(clippingId, new ClippingDto { Id = clippingId, Content = ClippingContent, Type = ClippingType });
            _omniServiceWrapper.SimulateMessage(new OmniMessage { Type = "clipping_created", Payload = new Dictionary<string, string> { {"id", clippingId} }});
        }

        #endregion
    }
}