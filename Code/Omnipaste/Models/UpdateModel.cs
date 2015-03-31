namespace Omnipaste.Models
{
    using Omnipaste.Entities;
    using OmniUI.Models;

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