using Ninject;
using Ninject.Modules;
using OmniCommon.Interfaces;
using WindowsClipboard.Interfaces;

namespace WindowsClipboard
{
    public class WindowsClipboardModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IWindowsClipboard>().To<WindowsClipboard>().InSingletonScope();
            Kernel.Bind<ILocalClipboard>().ToMethod(context => Kernel.Get<IWindowsClipboard>());
            Kernel.Bind<IWindowsClipboardWrapper>().To<WindowsClipboardWrapper>();
        }
    }
}
