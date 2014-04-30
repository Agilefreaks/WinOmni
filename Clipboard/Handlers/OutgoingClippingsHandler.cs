using System;
using WindowsClipboard.Interfaces;
using OmniCommon.DataProviders;
using OmniCommon.Models;
using OmniCommon.Services;

namespace Clipboard.Handlers
{
    public class OutgoingClippingsHandler : IOutgoingClippingHandler
    {
        private readonly IClippingsAPI _clippingsAPI;

        private readonly IConfigurationProvider _configurationProvider;

        private IDisposable _clippingsSubscription;

        public IWindowsClipboard WindowsClipboard { get; set; }

        public OutgoingClippingsHandler(IWindowsClipboard windowsClipboard, IClippingsAPI clippingsAPI, IConfigurationProvider configurationProvider)
        {
            _clippingsAPI = clippingsAPI;
            _configurationProvider = configurationProvider;
            WindowsClipboard = windowsClipboard;
        }

        public void Start()
        {
            _clippingsSubscription = WindowsClipboard.Subscribe(this);
        }

        public void Stop()
        {
            _clippingsSubscription.Dispose();
        }

        public void OnNext(ClipboardData value)
        {
            _clippingsAPI.PostClipping(_configurationProvider["deviceIdentifier"], value.GetData());
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