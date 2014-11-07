namespace Clipboard.Handlers
{
    using System;
    using OmniCommon.Models;

    public interface IOmniClipboardHandler : IClipboard, IObserver<OmniMessage>
    {
        void Start(IObservable<OmniMessage> observable);

        void Stop();
    }
}