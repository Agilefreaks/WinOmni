using System;
using System.Windows.Forms;

namespace WindowsClipboard.Interfaces
{
    public delegate bool MessageHandler(ref Message status);

    public interface IDelegateClipboardMessageHandling
    {
        event MessageHandler HandleClipboardMessage;

        IntPtr GetHandle();
    }
}