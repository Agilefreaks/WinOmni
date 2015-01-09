namespace Omnipaste.Services.Repositories
{
    using Omnipaste.Models;

    public class ClippingRepository : InMemoryRepository<ClippingModel>, IClippingRepository
    {
        protected override bool IsMatch(ClippingModel item, object id)
        {
            return Equals(item.UniqueId, id);
        }
    }
}