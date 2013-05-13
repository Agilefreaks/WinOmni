using Ninject.Modules;
using WindowsClipboard.Interfaces;

namespace WindowsClipboard
{
    public class WindowsClipboardModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IWindowsClipboard>().To<WindowsClipboard>();
            Kernel.Bind<IWindowsClipboardWrapper>().To<WindowsClipboardWrapper>();
        }
    }
}
