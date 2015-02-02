namespace Omnipaste.Models
{
    using System;
    using OmniCommon.Models;

    public class UserContactInfo : ContactInfo
    {
        public UserContactInfo(UserInfo userInfo = null)
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