using Ninject.Modules;

namespace ClipboardWrapper
{
    public class ClipboardWrapperModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IClipboardManager>().To<ClipboardManager>();
        }
    }
}
