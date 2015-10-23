namespace OmniUI.Framework.Models
{
    using System;
    using OmniUI.Framework.Entities;

    public interface IModel<T> : IModel
        where T : Entity
    {
        new T BackingEntity { get; set; }
    }

    public interface IModel
    {
        IEntity BackingEntity { get; set; }

        string Id { get; set; }

        bool IsDeleted { get; set; }

        DateTime Time { get; set; }

        string UniqueId { get; set; }

        bool WasViewed { get; set; }
    }
}