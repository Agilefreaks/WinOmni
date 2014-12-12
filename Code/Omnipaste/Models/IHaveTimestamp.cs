namespace Omnipaste.Models
{
    using System;

    public interface IHaveTimestamp
    {
        DateTime Time { get; }
    }
}