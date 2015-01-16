namespace OmniDebug.Services
{
    using System;
    using Clipboard.API.Resources.v1;
    using Clipboard.Models;

    public class ClippingsWrapper : ResourceWrapperBase<Clipping>, IClippingsWrapper
    {
        private readonly IClippings _clippings;

        public ClippingsWrapper(IClippings originalResource)
            : base(originalResource)
        {
            _clippings = originalResource;
        }

        public IObservable<Clipping> Create(string deviceId, string content)
        {
            return _clippings.Create(deviceId, content);
        }
    }
}