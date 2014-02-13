using Caliburn.Micro;
using OmniCommon.Interfaces;
using OmniCommon.Models;

namespace WindowsClipboard.Interfaces
{
    public interface IWindowsClipboard : ILocalClipboard, IHandle<Clipping>
    {
        IWindowsClipboardWrapper WindowsClipboardWrapper { get; set; }
    }
}