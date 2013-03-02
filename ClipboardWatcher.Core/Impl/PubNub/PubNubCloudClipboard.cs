using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ClipboardWatcher.Core.Impl.PubNub
{
    public class PubNubCloudClipboard : ICloudClipboard
    {
        private readonly string _channel;
        private readonly Pubnub _pubnub;

        public event EventHandler<ClipboardEventArgs> DataReceived;

        public PubNubCloudClipboard()
        {
            _channel = ConfigurationManager.AppSettings["channel"];
            _pubnub = new Pubnub(ConfigurationManager.AppSettings["publish-key"],
                     ConfigurationManager.AppSettings["subscribe-key"],
                     ConfigurationManager.AppSettings["secret-key"],
                     string.Empty,
                     true);
            _pubnub.subscribe(_channel, HandleMessageReceived);
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
            if (DataReceived != null)
            {
                DataReceived(this, e);
            }
        }

        private void HandleMessageReceived(object receivedMessage)
        {
            var dataOject = ((IEnumerable<object>) receivedMessage).First();
            var value = ((Newtonsoft.Json.Linq.JValue) dataOject).Value;

            OnDataReceived(new ClipboardEventArgs { Data = value.ToString() });
        }
    }
}
