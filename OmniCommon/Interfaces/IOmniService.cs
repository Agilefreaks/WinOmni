namespace OmniCommon.Interfaces
{
    using System.Threading.Tasks;

    public interface IOmniService : IDataReceiver
    {
        ILocalClipboard LocalClipboard { get; }

        IOmniClipboard OmniClipboard { get; }

        Task<bool> Start();

        void Stop();
    }
}