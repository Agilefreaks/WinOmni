namespace OmniCommon.Services
{
    using System;
    using OmniCommon.Interfaces;

    public class DateTimeService : IDateTimeService
    {
        public DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }
    }
}
