namespace Omnipaste.Framework.Models
{
    using Omnipaste.Framework.Entities;
    using OmniUI.Framework.Models;

    public class UpdateModel : Model<UpdateEntity>
    {
        public UpdateModel(UpdateEntity backingEntity)
            : base(backingEntity)
        {
        }

        public bool WasInstalled
        {
            get
            {
                return BackingEntity.WasInstalled;
            }
        }

        public string ReleaseLog
        {
            get
            {
                return BackingEntity.ReleaseLog;
            }

        }
    }
}