using System;
using OmniCommon;
using WindowsClipboard.Interfaces;

namespace WindowsClipboard
{
    public class WindowsClipboard : IWindowsClipboard
    {
        private IWindowsClipboardWrapper _windowsClipboardWrapper;
        public event EventHandler<ClipboardEventArgs> DataReceived;

        public IWindowsClipboardWrapper WindowsClipboardWrapper
        {
            get
            {
                return _windowsClipboardWrapper;
            }
            set
            {
                HookClipboardAdapter(_windowsClipboardWrapper, value);
                _windowsClipboardWrapper = value;
            }
        }

        public bool Initialize()
        {
            WindowsClipboardWrapper.StartWatchingClipboard();

            return true;
        }

        public void Dispose()
        {
            WindowsClipboardWrapper.StopWatchingClipboard();
        }

        public void SendData(string data)
        {
            WindowsClipboardWrapper.SetData(data);
        }

        private void HookClipboardAdapter(IWindowsClipboardWrapper windowsClipboardWrapper, IWindowsClipboardWrapper value)
        {
            if (windowsClipboardWrapper != null)
            {
                windowsClipboardWrapper.DataReceived -= ClipboardAdapterOnDataReceived;
            }

            if (value != null)
            {
                value.DataReceived += ClipboardAdapterOnDataReceived;
            }
        }

        private void ClipboardAdapterOnDataReceived(object sender, ClipboardEventArgs clipboardEventArgs)
        {
            DataReceived(this, clipboardEventArgs);
        }
    }
}