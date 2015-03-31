namespace Omnipaste.Services.Repositories
{
    using Omnipaste.Entities;
    using Omnipaste.Models;

    public class ClippingRepository : SecurePermanentRepository<ClippingEntity>, IClippingRepository
    {
        public ClippingRepository()
            : base("clippings")
        {
        }
    }
}
