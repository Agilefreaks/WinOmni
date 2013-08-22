namespace OmniCommon.Interfaces
{
    using System.Collections.Generic;
    using OmniCommon.Domain;

    using System.Threading.Tasks;

    public interface IOmniService : ICanReceiveData
    {
        ILocalClipboard LocalClipboard { get; }

        IOmniClipboard OmniClipboard { get; }

        IList<Clipping> GetClippings();

        Task<bool> Start();

        void Stop();
    }
}