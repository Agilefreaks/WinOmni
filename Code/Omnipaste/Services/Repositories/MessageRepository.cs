namespace Omnipaste.Services.Repositories
{
    using Omnipaste.Models;

    public class MessageRepository : InMemoryRepository<Message>, IMessageRepository
    {
        protected override bool IsMatch(Message item, object id)
        {
            return Equals(item.UniqueId, id);
        }
    }
}