namespace Omnipaste.Services
{
    using System.Diagnostics;

    public interface IProcessHelper
    {
        void Start(string processName);

        void Start(ProcessStartInfo processInfo);
    }
}
