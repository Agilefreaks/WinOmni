namespace Omnipaste.Services.Providers
{
    using Omnipaste.Models;
    using Omnipaste.Services.Repositories;

    public interface IConversationProvider
    {
        IConversationContext ForContact(ContactInfo contactInfo);
    }
}