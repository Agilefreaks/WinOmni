﻿namespace OmniUI.Framework.Models
{
    using System;
    using Caliburn.Micro;
    using OmniUI.Framework.Entities;

    public interface IModel
    {
        IEntity BackingEntity { get; set; }

        string Id { get; set; }

        bool IsDeleted { get; set; }

        DateTime Time { get; set; }

        string UniqueId { get; set; }

        bool WasViewed { get; set; }
    }

    public interface IModel<T> : IModel
        where T : Entity
    {
        new T BackingEntity { get; set; }
    }

    public class Model : PropertyChangedBase, IModel
    {
        public Model(IEntity backingEntity)
        {
            BackingEntity = backingEntity;
        }

        #region IModel Members

        public IEntity BackingEntity { get; set; }

        public string Id
        {
            get
            {
                return BackingEntity.Id;
            }

            set
            {
                BackingEntity.Id = value;
            }
        }

        public bool IsDeleted
        {
            get
            {
                return BackingEntity.IsDeleted;
            }
            set
            {
                BackingEntity.IsDeleted = value;
                NotifyOfPropertyChange();
            }
        }

        public DateTime Time
        {
            get
            {
                return BackingEntity.Time;
            }
            set
            {
                BackingEntity.Time = value;
            }
        }

        public string UniqueId
        {
            get
            {
                return BackingEntity.UniqueId;
            }
            set
            {
                BackingEntity.UniqueId = value;
            }
        }

        public bool WasViewed
        {
            get
            {
                return BackingEntity.WasViewed;
            }
            set
            {
                BackingEntity.WasViewed = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion
    }

    public class Model<T> : Model, IModel<T>
        where T : Entity
    {
        public Model(T backingEntity)
            : base(backingEntity)
        {
            BackingEntity = backingEntity;
        }

        #region IModel<T> Members

        public new T BackingEntity { get; set; }

        #endregion
    }
}