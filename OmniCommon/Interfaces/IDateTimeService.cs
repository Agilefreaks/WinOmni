using System;

namespace OmniCommon.Interfaces
{
    public interface IDateTimeService
    {
        DateTime UtcNow { get; }
    }
}