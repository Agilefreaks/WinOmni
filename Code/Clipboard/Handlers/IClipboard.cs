namespace Clipboard.Handlers
{
    using System;
    using Clipboard.Dto;

    public interface IClipboard
    {
        void PostClipping(ClippingDto clippingDto);

        IObservable<ClippingDto> Clippings { get; }
    }
}