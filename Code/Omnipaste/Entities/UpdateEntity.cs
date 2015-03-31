namespace Omnipaste.Entities
{
    using System;
    using OmniCommon.Helpers;
    using OmniUI.Entities;

    public class UpdateEntity : Entity
    {
        public bool WasInstalled { get; set; }

        public string ReleaseLog { get; set; }

        public UpdateEntity()
        {
            UniqueId = Guid.NewGuid().ToString();
            Time = TimeHelper.UtcNow;
        }
    }
}