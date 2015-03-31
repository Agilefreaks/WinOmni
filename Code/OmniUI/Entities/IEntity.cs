namespace OmniUI.Entities
{
    using System;

    public interface IEntity
    {
        string Id { get; set; }

        bool IsDeleted { get; set; }

        DateTime Time { get; set; }

        string UniqueId { get; set; }

        bool WasViewed { get; set; }
    }
}