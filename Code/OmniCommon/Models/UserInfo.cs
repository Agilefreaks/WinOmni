namespace OmniCommon.Models
{
    using System;

    [Serializable]
    public class UserInfo
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ImageUrl { get; set; }

        public string Email { get; set; }

        public DateTime ContactsUpdatedAt { get; set; }
    }
}
