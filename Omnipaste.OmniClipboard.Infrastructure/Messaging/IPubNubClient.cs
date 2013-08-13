using System;
using System.ComponentModel;

namespace Omnipaste.OmniClipboard.Infrastructure.Messaging
{
    public interface IPubNubClient : INotifyPropertyChanged
    {
        void Subscribe<T>(string channel, Action<T> userCallback, Action<T> connectCallback);

        void EndPendingRequests();

        void Unsubscribe(string channel, Action<object> userCallback, Action<object> connectCallback, Action<object> disconnectCallback);

        bool Publish(string channel, object message, Action<object> userCallback);
    }
}