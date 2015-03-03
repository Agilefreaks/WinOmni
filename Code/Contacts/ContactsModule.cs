namespace Contacts
{
    using Contacts.Api.Resources.v1;
    using Contacts.Handlers;
    using Ninject;
    using Ninject.Modules;
    using OmniCommon.Interfaces;

    public class ContactsModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IContacts>().To<Contacts>().InSingletonScope();

            Kernel.Bind<IContactCreatedHandler>().To<ContactCreatedHandler>().InSingletonScope();
            Kernel.Bind<IContactsUpdatedHandler>().To<ContactsUpdatedHandler>().InSingletonScope();

            Kernel.Bind<IHandler>().ToMethod(context => context.Kernel.Get<IContactCreatedHandler>());
            Kernel.Bind<IHandler>().ToMethod(context => context.Kernel.Get<IContactsUpdatedHandler>());
        }
    }
}