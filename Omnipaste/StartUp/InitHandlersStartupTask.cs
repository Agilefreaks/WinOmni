using Ninject;
using OmniCommon.Interfaces;
using Omnipaste.Framework;

namespace Omnipaste.StartUp
{
    public class InitHandlersStartupTask : IStartupTask
    {
        [Inject]
        public IKernel Kernel { get; set; }

        public void Startup()
        {
            Kernel.Get<IOmniServiceHandler>();
        }
    }
}
