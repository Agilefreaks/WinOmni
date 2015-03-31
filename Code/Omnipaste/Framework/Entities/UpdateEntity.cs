namespace Omnipaste.Framework.Entities
{
    using System;
    using OmniCommon.Helpers;
    using OmniUI.Framework.Entities;

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