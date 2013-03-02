using Ninject.Modules;

namespace ClipboardWatcher
{
    public class MainModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<MainForm>().To<MainForm>();
        }
    }
}
