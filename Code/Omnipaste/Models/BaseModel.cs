namespace Omnipaste.Models
{
    using System;
    using OmniCommon.Helpers;

    public abstract class BaseModel : IModel
    {
        #region Constructors and Destructors

        protected BaseModel()
        {
            UniqueId = Guid.NewGuid().ToString();
            Time = TimeHelper.UtcNow;
        }

        #endregion

        #region Public Properties

        public bool IsDeleted { get; set; }

        public DateTime Time { get; set; }

        public string UniqueId { get; set; }

        public bool WasViewed { get; set; }

        #endregion
    }
}