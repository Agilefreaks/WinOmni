using System;
using System.Windows.Forms;

namespace WindowsClipboard
{
    public interface IClipboardWrapper : IDisposable
    {
        ClipboardMessageHandleResult HandleClipboardMessage(Message message);

        void Initialize(IntPtr handle);

        void SendToClipboard(string data);
    }
}