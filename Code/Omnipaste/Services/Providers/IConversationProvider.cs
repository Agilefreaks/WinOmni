namespace Omnipaste.Services.Providers
{
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    public interface IConversationProvider
    {
        IConversationContext ForContact(ContactEntity contactEntity);

        IConversationContext All();
    }
}