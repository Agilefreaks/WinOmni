namespace OmniUI.Framework.Entities
{
    using System;
    using OmniCommon.Helpers;

    public interface IEntity
    {
        string Id { get; set; }

        bool IsDeleted { get; set; }

        DateTime Time { get; set; }

        string UniqueId { get; set; }

        bool WasViewed { get; set; }
    }

    public abstract class Entity : IEntity
    {
        private DateTime _time;

        protected Entity()
        {
            UniqueId = Guid.NewGuid().ToString();
            Time = TimeHelper.UtcNow;
        }

        #region IEntity Members

        public string Id { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value.ToUniversalTime();
            }
        }

        public string UniqueId { get; set; }

        public bool WasViewed { get; set; }

        #endregion
    }
}