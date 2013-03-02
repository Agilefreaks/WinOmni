using Ninject.Modules;

namespace ClipboardWrapper
{
    public class ClipboardWrapperModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IClipboardWrapper>().To<ClipboardWrapper>();
            Kernel.Bind<IClipboardAdapter>().To<ClipboardAdapter>();
        }
    }
}
