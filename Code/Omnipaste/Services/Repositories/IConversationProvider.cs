namespace Omnipaste.Services.Repositories
{
    using Omnipaste.Models;

    public interface IConversationProvider
    {
        IConversationContext ForContact(ContactInfo contactInfo);
    }
}