namespace Omnipaste.Services
{
    using System;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using OmniUI.Entities;

    public class UpdateInfo : Entity
    {
        public bool WasInstalled { get; set; }

        public string ReleaseLog { get; set; }

        public UpdateInfo()
        {
            UniqueId = Guid.NewGuid().ToString();
            Time = TimeHelper.UtcNow;
        }
    }
}