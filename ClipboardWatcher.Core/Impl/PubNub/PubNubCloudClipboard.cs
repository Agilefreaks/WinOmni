using System;
using System.Configuration;

namespace ClipboardWatcher.Core.Impl.PubNub
{
    public class PubNubCloudClipboard : ICloudClipboard
    {
        private readonly string _channel;
        private readonly Pubnub _pubnub;

        public event EventHandler<ClipboardEventArgs> DataReceived;

        public PubNubCloudClipboard()
        {
            _pubnub = new Pubnub(ConfigurationManager.AppSettings["publish-key"],
                                 ConfigurationManager.AppSettings["subscribe-key"],
                                 ConfigurationManager.AppSettings["secret-key"],
                                 string.Empty,
                                 true);
        }

        public PubNubCloudClipboard(string channel)
        {
            _channel = channel;
            _pubnub.subscribe(_channel, o => OnDataReceived(new ClipboardEventArgs { Data = o as string }));
        }

        public void Copy(string str)
        {
            _pubnub.publish(_channel, str, o => { });
        }

        public void Dispose()
        {
            _pubnub.EndPendingRequests();
        }

        protected virtual void OnDataReceived(ClipboardEventArgs e)
        {
            var handler = DataReceived;
            if (handler != null) handler(this, e);
        }
    }
}
