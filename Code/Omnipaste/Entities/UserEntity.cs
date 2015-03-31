namespace Omnipaste.Entities
{
    using System;
    using OmniCommon.Models;

    public class UserEntity : ContactEntity
    {
        public UserEntity(UserInfo userInfo = null)
        {
            if (userInfo == null)
            {
                return;
            }
            FirstName = userInfo.FirstName;
            LastName = userInfo.LastName;
            ImageUri = string.IsNullOrWhiteSpace(userInfo.ImageUrl) ? null : new Uri(userInfo.ImageUrl);
        }
    }
}