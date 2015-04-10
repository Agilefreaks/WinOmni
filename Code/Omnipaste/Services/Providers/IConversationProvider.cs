namespace Omnipaste.Services.Providers
{
    using Omnipaste.Framework.Entities;
    using Omnipaste.Services.Repositories;

    public interface IConversationProvider
    {
        IConversationContext ForContact(ContactEntity contactEntity);

        IConversationContext All();
    }
}