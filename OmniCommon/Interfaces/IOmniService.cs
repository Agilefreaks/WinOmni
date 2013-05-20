namespace OmniCommon.Interfaces
{
    using System.Threading.Tasks;

    public interface IOmniService
    {
        ILocalClipboard LocalClipboard { get; }

        IOmniClipboard OmniClipboard { get; }

        Task Start();

        void Stop();
    }
}