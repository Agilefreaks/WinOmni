using Caliburn.Micro;
using OmniApi.Models;

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

        [Inject]
        public IEventAggregator EventAggregator { get; set; }

        public override Task<bool> Initialize()
        {
            EventAggregator.Subscribe(this);

            return Task<bool>.Factory.StartNew(() =>
                    {
                        WindowsClipboardWrapper.StartWatchingClipboard();
                        return true;
                    });
        }

        public override void Dispose()
        {
            WindowsClipboardWrapper.StopWatchingClipboard();
            EventAggregator.Unsubscribe(this);
        }

        //TODO: this should not be public or should not exist at all. This can be done in the Handle clipping method
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
            NotifyReceivers(new ClipboardData(this, clipboardEventArgs.Data));
        }

        public void Handle(Clipping clipping)
        {
            PutData(clipping.Content);
        }
    }
}