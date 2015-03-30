namespace OmniUI.Models
{
    using System;

    public interface IModel
    {
        string Id { get; set; }

        bool IsDeleted { get; set; }

        DateTime Time { get; set; }

        string UniqueId { get; set; }

        bool WasViewed { get; set; }
    }
}