using System;
using WindowsClipboard.Interfaces;
using OmniCommon.Models;
using OmniCommon.Services;

namespace Clipboard.Handlers
{
    class OutgoingClippingsHandler : IOutgoingClippingHandler
    {
        private readonly IClippingsAPI _clippingsAPI;

        private IDisposable _clippingsSubscription;

        public IWindowsClipboard WindowsClipboard { get; set; }

        public OutgoingClippingsHandler(IWindowsClipboard windowsClipboard, IClippingsAPI clippingsAPI)
        {
            _clippingsAPI = clippingsAPI;
            WindowsClipboard = windowsClipboard;
        }

        public void Start()
        {
            _clippingsSubscription = WindowsClipboard.Clippings.Subscribe(this);
        }

        public void Stop()
        {
            _clippingsSubscription.Dispose();
        }

        public void OnNext(ClipboardData value)
        {
            _clippingsAPI.PostClipping(new Clipping("eMailAddress", value.GetData()));
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}