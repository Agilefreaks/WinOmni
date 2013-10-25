namespace OmniClipboard.Messaging
{
    using System;
    using System.ComponentModel;

    public interface IPubNubClient : INotifyPropertyChanged
    {
        void Subscribe<T>(string channel, Action<T> userCallback, Action<T> connectCallback, Action<T> errorCallback);

        void Unsubscribe<T>(string channel, Action<T> userCallback, Action<T> connectCallback, Action<T> disconnectCallback, Action<T> errorCallback);

        bool Publish<T>(string channel, object message, Action<T> userCallback, Action<T> errorCallback);

        void EndPendingRequests();
    }
}