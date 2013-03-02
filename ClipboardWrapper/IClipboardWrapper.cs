using System.Windows.Forms;

namespace ClipboardWrapper
{
    public interface IClipboardWrapper
    {
        ClipboardMessageHandleResult HandleClipboardMessage(Message message);
    }
}