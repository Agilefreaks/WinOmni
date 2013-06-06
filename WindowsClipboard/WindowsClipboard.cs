namespace WindowsClipboard
{
    using System.Threading.Tasks;
    using Ninject;
    using OmniCommon.Services;
    using global::WindowsClipboard.Interfaces;

    public class WindowsClipboard : ClipboardBase, IWindowsClipboard
    {
        private IWindowsClipboardWrapper _windowsClipboardWrapper;

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

        public override Task<bool> Initialize()
        {
            return Task<bool>.Factory.StartNew(() =>
                    {
                        WindowsClipboardWrapper.StartWatchingClipboard();
                        return true;
                    });
        }

        public override void Dispose()
        {
            WindowsClipboardWrapper.StopWatchingClipboard();
        }

        public override void PutData(string data)
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
            OnDataReceived(new ClipboardData(this, clipboardEventArgs.Data));
        }
    }
}