namespace Clipboard.Handlers
{
    using System;
    using Clipboard.Models;

    public interface IClipboard
    {
        void PostClipping(Clipping clipping);

        IObservable<Clipping> Clippings { get; }
    }
}