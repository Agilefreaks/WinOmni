using OmniCommon.Interfaces;

namespace WindowsClipboard.Interfaces
{
    public interface IWindowsClipboard : ILocalClipboard
    {
        IWindowsClipboardWrapper WindowsClipboardWrapper { get; set; }
    }
}