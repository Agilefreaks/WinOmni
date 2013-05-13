using OmniCommon.Interfaces;

namespace PubNubClipboard
{
    public interface IPubNubClipboard : IOmniClipboard
    {
        bool IsInitialized { get; }

        string Channel { get; }
    }
}
