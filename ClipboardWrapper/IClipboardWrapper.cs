using System;
using System.Windows.Forms;

namespace ClipboardWrapper
{
    public interface IClipboardWrapper
    {
        ClipboardMessageHandleResult HandleClipboardMessage(Message message);

        void RegisterClipboardViewer(IntPtr handle);

        void UnRegisterClipboardViewer(IntPtr handle);
    }
}