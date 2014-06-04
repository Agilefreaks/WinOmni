namespace WindowsClipboard
{
    using Ninject.Modules;
    using WindowsClipboard.Interfaces;

    public class WindowsClipboardModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IWindowsClipboardWrapper>().To<WindowsClipboardWrapper>();
        }
    }
}
