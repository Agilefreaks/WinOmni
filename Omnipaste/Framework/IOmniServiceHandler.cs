using Caliburn.Micro;
using OmniCommon.EventAggregatorMessages;

namespace Omnipaste.Framework
{
    public interface IOmniServiceHandler : IHandleWithTask<StartOmniServiceMessage>, IHandle<StopOmniServiceMessage>
    {
        void Init();
    }
}