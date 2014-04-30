using System;
using Ninject;
using OmniCommon.Services;

namespace Clipboard.Handlers
{
    public interface IOutgoingClippingHandler : IObserver<ClipboardData>, IStartable
    {
    }
}