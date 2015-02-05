namespace Omnipaste.Models
{
    using System;

    public interface IModel
    {
        bool IsDeleted { get; set; }

        DateTime Time { get; set; }

        string UniqueId { get; set; }

        bool WasViewed { get; set; }
    }
}