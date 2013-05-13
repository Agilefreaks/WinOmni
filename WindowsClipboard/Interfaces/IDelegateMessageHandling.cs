using System.Windows.Forms;

namespace WindowsClipboard.Interfaces
{
    public delegate bool MessageHandler(ref Message status);

    public interface IDelegateMessageHandling
    {
        event MessageHandler HandleMessage;
    }
}