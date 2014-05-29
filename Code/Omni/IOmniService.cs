namespace Omni
{
    using System.Threading.Tasks;

    public interface IOmniService
    {
        Task<bool> Start(string communicationChannel = null);

        void Stop();
    }
}