namespace Omnipaste.Services.Repositories
{
    using Omnipaste.Models;

    public class MessageRepository : InMemoryRepository<Message>, IMessageRepository
    {
    }
}