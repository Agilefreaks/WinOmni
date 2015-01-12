namespace Omnipaste.Models
{
    using System;

    public class BaseModel
    {
        #region Constructors and Destructors

        public BaseModel()
        {
            UniqueId = Guid.NewGuid().ToString();
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