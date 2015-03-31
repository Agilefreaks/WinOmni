namespace Clipboard.Handlers
{
    using System;
    using Clipboard.Dto;
    using OmniCommon.Interfaces;

    public interface IClipboardHandler : IHandler, IObservable<ClippingDto>
    {
    }
}