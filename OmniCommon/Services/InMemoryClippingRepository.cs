namespace OmniCommon.Services
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using OmniCommon.Domain;
    using OmniCommon.Interfaces;

    public class InMemoryClippingRepository : IClippingRepository
    {
        private readonly IList<Clipping> _clippings;

        public IDateTimeService DateTimeService { get; set; }

        public InMemoryClippingRepository(IDateTimeService dateTimeService)
        {
            DateTimeService = dateTimeService;
            _clippings = new List<Clipping>();
        }

        public IList<Clipping> GetAll()
        {
            var now = DateTimeService.UtcNow;
            return _clippings
                .Where(c => c.DateCreated > now.Subtract(TimeSpan.FromHours(24)))
                .OrderByDescending(c => c.DateCreated)
                .ToList();
        }

        public void Save(Clipping clip)
        {
            clip.DateCreated = DateTimeService.UtcNow;

            _clippings.Add(clip);
        }
    }
}
