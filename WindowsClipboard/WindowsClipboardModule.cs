using Ninject.Modules;
using WindowsClipboard.Interfaces;

namespace WindowsClipboard
{
    public class WindowsClipboardModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IWindowsClipboard>().To<WindowsClipboard>().InSingletonScope();
            Kernel.Bind<IWindowsClipboardWrapper>().To<WindowsClipboardWrapper>();
        }
    }
}
