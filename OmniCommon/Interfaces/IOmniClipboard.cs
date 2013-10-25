namespace OmniCommon.Interfaces
{
    public interface IOmniClipboard : IClipboard
    {
        string Channel { get; }
    }
}