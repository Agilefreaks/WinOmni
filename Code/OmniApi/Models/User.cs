namespace OmniApi.Models
{
    using System;

    public class User
    {
        public User()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            ImageUrl = string.Empty;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string ImageUrl { get; set; }

        public bool ViaOmnipaste { get; set; }

        public DateTime? ContactsUpdatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}