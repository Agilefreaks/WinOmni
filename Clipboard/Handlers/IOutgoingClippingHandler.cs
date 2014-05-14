using System;
using WindowsClipboard;
using Ninject;
using OmniCommon.Services;

namespace Clipboard.Handlers
{
    public interface IOutgoingClippingHandler : IObserver<ClipboardData>, IStartable
    {
    }
}