using Caliburn.Micro;
using OmniApi.Models;
using OmniCommon.Interfaces;

namespace WindowsClipboard.Interfaces
{
    public interface IWindowsClipboard : ILocalClipboard, IHandle<Clipping>
    {
        IWindowsClipboardWrapper WindowsClipboardWrapper { get; set; }
    }
}