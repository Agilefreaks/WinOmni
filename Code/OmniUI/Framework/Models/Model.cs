namespace OmniUI.Framework.Models
{
    using System;
    using Caliburn.Micro;
    using OmniUI.Framework.Entities;

    public abstract class Model : PropertyChangedBase, IModel
    {
        protected Model(IEntity backingEntity)
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

    public abstract class Model<T> : Model, IModel<T>
        where T : Entity
    {
        protected Model(T backingEntity)
            : base(backingEntity)
        {
            BackingEntity = backingEntity;
        }

        #region IModel<T> Members

        public new T BackingEntity { get; set; }

        #endregion
    }
}