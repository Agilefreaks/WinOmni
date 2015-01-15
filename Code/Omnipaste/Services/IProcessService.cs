namespace Omnipaste.Services
{
    using System.Diagnostics;

    public interface IProcessService
    {
        void Start(ProcessStartInfo processInfo);
    }
}
