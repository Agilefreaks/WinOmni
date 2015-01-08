namespace Omnipaste.Models
{
    using System;
    
    public class BaseModel
    {
        public string UniqueId { get; set; }

        public bool IsDeleted { get; set; }

        public bool WasViewed { get; set; }

        public DateTime Time { get; set; }
    }
}