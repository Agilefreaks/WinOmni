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
            Kernel.Bind<ILocalClipboardHandler>().To<LocalClipboardHandler>().InSingletonScope();
            Kernel.Bind<IOmniClipboardHandler>().To<ClippingCreatedHandler>().InSingletonScope();
            Kernel.Bind<IHandler, IClipboardHandler>().To<ClipboardHandler>().InSingletonScope();

            Kernel.Bind<IClippings>().To<Clippings>();
        }
    }
}