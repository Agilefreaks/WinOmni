using System;

namespace ClipboardWatcher.Core
{
    public interface IPubNubCloudClipboard : ICloudClipboard, IDisposable
    {
        bool IsInitialized { get; }

        void Initialize();
    }
}