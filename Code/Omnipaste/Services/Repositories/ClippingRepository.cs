namespace Omnipaste.Services.Repositories
{
    using Omnipaste.Framework.Entities;

    public class ClippingRepository : SecurePermanentRepository<ClippingEntity>, IClippingRepository
    {
        public ClippingRepository()
            : base("clippings")
        {
        }
    }
}
