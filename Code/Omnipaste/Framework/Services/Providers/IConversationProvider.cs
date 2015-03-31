namespace Omnipaste.Framework.Services.Providers
{
    using Omnipaste.Framework.Entities;
    using Omnipaste.Framework.Services.Repositories;

    public interface IConversationProvider
    {
        IConversationContext ForContact(ContactEntity contactEntity);

        IConversationContext All();
    }
}