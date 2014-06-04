namespace Clipboard.Handlers
{
    using System;
    using Clipboard.Models;

    public interface IClipboard : IObservable<Clipping>, IDisposable
    {
        void PostClipping(Clipping clipping);
    }
}