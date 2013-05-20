namespace WindowsClipboard
{
    using System;
    using System.Threading.Tasks;
    using Ninject;
    using OmniCommon;

    using global::WindowsClipboard.Interfaces;

    public class WindowsClipboard : IWindowsClipboard
    {
        private IWindowsClipboardWrapper _windowsClipboardWrapper;

        public event EventHandler<ClipboardEventArgs> DataReceived;

        [Inject]
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

        public Task<bool> Initialize()
        {
            return Task<bool>.Factory.StartNew(() =>
                    {
                        WindowsClipboardWrapper.StartWatchingClipboard();
                        return true;
                    });
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