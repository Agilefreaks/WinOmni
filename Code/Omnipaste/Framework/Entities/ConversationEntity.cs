namespace Omnipaste.Framework.Entities
{
    using System;
    using OmniUI.Framework.Entities;

    public abstract class ConversationEntity : Entity
    {
        private string _contactUniqueId;

        public String ContactUniqueId
        {
            get
            {
                return _contactUniqueId ?? ContactInfoUniqueId;
            }
            set
            {
                _contactUniqueId = value;
            }
        }

        // Compatibility property, for the entities that already saved it.
        public String ContactInfoUniqueId { get; set; }
    }
}