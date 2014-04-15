using System;
using Caliburn.Micro;
using Ninject;
using OmniCommon.Interfaces;
using OmniCommon.Models;
using OmniCommon.Services;

namespace WindowsClipboard.Interfaces
{
    public interface IWindowsClipboard : IClipboard, IHandle<Clipping>, IStartable
    {
        IWindowsClipboardWrapper WindowsClipboardWrapper { get; set; }

        IEventAggregator EventAggregator { get; set; }

        IObservable<ClipboardData> Clippings { get; set; }
    }
}