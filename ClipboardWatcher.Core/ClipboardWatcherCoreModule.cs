using ClipboardWatcher.Core.Impl.PubNub;
using Ninject.Modules;

namespace ClipboardWatcher.Core
{
    public class ClipboardWatcherCoreModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<ICloudClipboard>().To<PubNubCloudClipboard>();
        }
    }
}
