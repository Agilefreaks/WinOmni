using System;
using WindowsClipboard.Interfaces;
using OmniCommon.Interfaces;
using OmniCommon.Services;

namespace Clipboard.Handlers
{
    public class OutgoingClippingsHandler : IOutgoingClippingHandler
    {
        private readonly IClippingsAPI _clippingsAPI;

        private readonly IConfigurationService _configurationService;

        private IDisposable _clippingsSubscription;

        public IWindowsClipboard WindowsClipboard { get; set; }

        public OutgoingClippingsHandler(IWindowsClipboard windowsClipboard, IClippingsAPI clippingsAPI, IConfigurationService configurationService)
        {
            _clippingsAPI = clippingsAPI;
            _configurationService = configurationService;
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
            _clippingsAPI.PostClipping(_configurationService.GetDeviceIdentifier(), value.GetData());
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