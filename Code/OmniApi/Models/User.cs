namespace OmniApi.Models
{
    public class User
    {
        #region Constructors and Destructors

        public User()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            ImageUrl = string.Empty;
        }

        #endregion

        #region Public Properties

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string ImageUrl { get; set; }

        #endregion
    }
}