namespace OmniCommon.Models
{
    using System;

    [Serializable]
    public class UserInfo : IUserInfoBuilder
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ImageUrl { get; set; }

        public string Email { get; set; }

        public DateTime ContactsUpdatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        #region IUserInfoBuilder Members

        IUserInfoBuilder IUserInfoBuilder.WithFirstName(string firstName)
        {
            FirstName = firstName;
            return this;
        }

        IUserInfoBuilder IUserInfoBuilder.WithLastName(string lastName)
        {
            LastName = lastName;
            return this;
        }

        IUserInfoBuilder IUserInfoBuilder.WithImageUrl(string imageUrl)
        {
            ImageUrl = imageUrl;
            return this;
        }

        IUserInfoBuilder IUserInfoBuilder.WithEmail(string email)
        {
            Email = email;
            return this;
        }

        IUserInfoBuilder IUserInfoBuilder.WithContactsUpdatedAt(DateTime contactsUpdatedAt)
        {
            ContactsUpdatedAt = contactsUpdatedAt;
            return this;
        }

        UserInfo IUserInfoBuilder.Build()
        {
            return this;
        }

        #endregion

        public UserInfo SetContactsUpdatedAt(DateTime contactsUpdatedAt)
        {
            ContactsUpdatedAt = contactsUpdatedAt;
            return this;
        }

        public static IUserInfoBuilder BeginBuild()
        {
            return new UserInfo();
        }
    }

    public interface IUserInfoBuilder
    {
        IUserInfoBuilder WithFirstName(String firstName);

        IUserInfoBuilder WithLastName(String lastName);

        IUserInfoBuilder WithImageUrl(String imageUrl);

        IUserInfoBuilder WithEmail(String email);

        IUserInfoBuilder WithContactsUpdatedAt(DateTime contactsUpdatedAt);

        UserInfo Build();
    }
}