using System;
using System.Threading.Tasks;
using WindowsClipboard.Interfaces;
using OmniCommon.Interfaces;
using OmniCommon.Models;
using OmniCommon.Services;
using RestSharp;

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
            var postClippingTask = _clippingsAPI.PostClipping(_configurationService.GetDeviceIdentifier(), value.GetData());
            postClippingTask.Wait();
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