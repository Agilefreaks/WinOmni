using Ninject;
using Ninject.Modules;
using WindowsClipboard.Interfaces;

namespace WindowsClipboard
{
    public class WindowsClipboardModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IWindowsClipboardWrapper>().To<WindowsClipboardWrapper>();
            Kernel.Bind<IWindowsClipboard, IStartable>().To<WindowsClipboard>().InSingletonScope();
        }
    }
}
