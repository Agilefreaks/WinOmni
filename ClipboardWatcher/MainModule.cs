﻿using ClipboardWatcher.Core;
using ClipboardWatcher.Core.Impl.PubNub;
using ClipboardWatcher.Core.Services;
using Ninject.Modules;

namespace ClipboardWatcher
{
    public class MainModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<MainForm>().To<MainForm>();
            Kernel.Bind<IConfigurationProvider>().To<DPAPIConfigurationProvider>().InSingletonScope();
            Kernel.Bind<ICloudClipboard>().To<PubNubCloudClipboard>().InSingletonScope();
        }
    }
}
