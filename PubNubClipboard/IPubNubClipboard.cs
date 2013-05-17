using OmniCommon.Interfaces;

namespace PubNubClipboard
{
    public interface IPubNubClipboard : IOmniClipboard
    {
        string Channel { get; }
    }
}
