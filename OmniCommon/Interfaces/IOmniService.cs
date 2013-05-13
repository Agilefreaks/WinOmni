namespace OmniCommon.Interfaces
{
    public interface IOmniService
    {
        ILocalClipboard LocalClipboard { get; }

        IOmniClipboard OmniClipboard { get; }

        void Start();

        void Stop();
    }
}