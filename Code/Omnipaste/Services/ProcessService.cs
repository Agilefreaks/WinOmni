namespace Omnipaste.Services
{
    using System.Diagnostics;

    public class ProcessService : IProcessService
    {
        public void Start(ProcessStartInfo processInfo)
        {
            Process.Start(processInfo);
        }
    }
}