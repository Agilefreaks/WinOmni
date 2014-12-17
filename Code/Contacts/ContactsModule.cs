namespace Contacts
{
    using Contacts.Api.Resources.v1;
    using Ninject.Modules;

    public class ContactsModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IContacts>().To<Contacts>().InSingletonScope();
        }
    }
}