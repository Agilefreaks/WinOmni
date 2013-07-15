using Caliburn.Micro;
using OmniCommon.EventAggregatorMessages;

namespace Omnipaste.Framework
{
    public interface IOmniServiceHandler : IHandle<StartOmniServiceMessage>, IHandle<StopOmniServiceMessage>
    {
    }
}