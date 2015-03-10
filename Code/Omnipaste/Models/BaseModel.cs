namespace Omnipaste.Models
{
    using System;
    using OmniCommon.Helpers;

    public abstract class BaseModel : IModel
    {
        private DateTime _time;

        protected BaseModel()
        {
            UniqueId = Guid.NewGuid().ToString();
            Time = TimeHelper.UtcNow;
        }

        #region IModel Members

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