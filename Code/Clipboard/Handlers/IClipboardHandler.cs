namespace Clipboard.Handlers
{
    using System;
    using Clipboard.Models;
    using OmniCommon.Interfaces;

    public interface IClipboardHandler : IHandler, IObservable<Clipping>
    {
    }
}