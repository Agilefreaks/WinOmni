namespace Omnipaste.Services.Repositories
{
    using Omnipaste.Models;

    public class ClippingRepository : SecurePermanentRepository<ClippingModel>, IClippingRepository
    {
        public ClippingRepository()
            : base("clippings")
        {
        }
    }
}
