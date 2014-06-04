using System;
using WindowsClipboard;
using Ninject;

namespace Clipboard.Handlers
{
    public interface IOutgoingClippingHandler : IObserver<ClipboardData>, IStartable
    {
    }
}