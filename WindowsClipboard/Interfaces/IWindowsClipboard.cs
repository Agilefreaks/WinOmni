using System;
using Caliburn.Micro;
using Ninject;

namespace WindowsClipboard.Interfaces
{
    public interface IWindowsClipboard : IHandle<ClipboardData>, IStartable, IObservable<ClipboardData>
    {
        IWindowsClipboardWrapper WindowsClipboardWrapper { get; set; }

        IEventAggregator EventAggregator { get; set; }

        IObservable<ClipboardData> Clippings { get; set; }
    }
}