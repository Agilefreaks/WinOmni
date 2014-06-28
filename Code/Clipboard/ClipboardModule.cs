namespace Clipboard
{
    using Clipboard.API.Resources.v1;
    using Clipboard.Handlers.WindowsClipboard;
    using Clipboard.Handlers;
    using Ninject.Modules;
    using OmniCommon.Interfaces;

    public class ClipboardModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IWindowsClipboardWrapper>().To<WindowsClipboardWrapper>().InSingletonScope();
            Kernel.Bind<ILocalClipboardHandler>().To<LocalClipboardsHandler>().InSingletonScope();
            Kernel.Bind<IOmniClipboardHandler>().To<OmniClipboardHandler>().InSingletonScope();
            Kernel.Bind<IHandler, IClipboadHandler>().To<ClipboardHandler>().InSingletonScope();

            Kernel.Bind<IClippings>().To<Clippings>();
        }
    }
}