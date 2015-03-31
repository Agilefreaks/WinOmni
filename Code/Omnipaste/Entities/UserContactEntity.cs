namespace Omnipaste.Models
{
    using System;
    using OmniCommon.Models;
    using Omnipaste.Entities;

    public class UserContactEntity : ContactEntity
    {
        public UserContactEntity(UserInfo userInfo = null)
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