namespace Contacts
{
    using Contacts.Api.Resources.v1;
    using Contacts.Handlers;
    using Ninject.Modules;
    using OmniCommon.Interfaces;

    public class ContactsModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IContacts>().To<Contacts>().InSingletonScope();

            Kernel.Bind<IHandler>().To<ContactsHandler>().InSingletonScope();
        }
    }
}