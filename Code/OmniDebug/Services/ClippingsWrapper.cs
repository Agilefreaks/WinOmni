namespace OmniDebug.Services
{
    using System;
    using Clipboard.API.Resources.v1;
    using Clipboard.Dto;

    public class ClippingsWrapper : ResourceWrapperBase<ClippingDto>, IClippingsWrapper
    {
        private readonly IClippings _clippings;

        public ClippingsWrapper(IClippings originalResource)
            : base(originalResource)
        {
            _clippings = originalResource;
        }

        public IObservable<ClippingDto> Create(string deviceId, string content)
        {
            return _clippings.Create(deviceId, content);
        }
    }
}