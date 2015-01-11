namespace Omnipaste.Services.Repositories
{
    using Omnipaste.Models;

    public class CallRepository : InMemoryRepository<Call>, ICallRepository
    {
        protected override bool IsMatch(Call item, object id)
        {
            return Equals(item.UniqueId, id);
        }
    }
}