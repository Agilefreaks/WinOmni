namespace Omnipaste.Models
{
    using System;
    using OmniCommon.Models;
    using OmniUI.Models;

    public class UserContactInfo  :ContactInfo
    {
        public UserContactInfo(UserInfo userInfo)
        {
            FirstName = userInfo.FirstName;
            LastName = userInfo.LastName;
            ImageUri = string.IsNullOrWhiteSpace(userInfo.ImageUrl) ? null : new Uri(userInfo.ImageUrl);
        }
    }
}