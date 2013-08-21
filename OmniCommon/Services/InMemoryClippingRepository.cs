namespace OmniCommon.Services
{
    using System.Collections.Generic;
    using OmniCommon.Domain;
    using OmniCommon.Interfaces;

    public class InMemoryClippingRepository : IClippingRepository
    {
        private readonly IList<Clipping> _clippings;

        public InMemoryClippingRepository()
        {
            _clippings = new List<Clipping>();
        }

        public IList<Clipping> GetAll()
        {
            return _clippings;
        }

        public void Save(Clipping clip)
        {
            _clippings.Add(clip);
        }
    }
}
