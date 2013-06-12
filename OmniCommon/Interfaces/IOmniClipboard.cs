namespace OmniCommon.Interfaces
{
    using Common.Logging;

    public interface IOmniClipboard : IClipboard
    {
        string Channel { get; }

        ILog Logger { get; set; }
    }
}