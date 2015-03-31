namespace Omnipaste.Models
{
    using Clipboard.Dto;
    using Omnipaste.Entities;
    using Omnipaste.Properties;
    using OmniUI.Models;

    public class ClippingModel : Model<ClippingEntity>
    {
        public ClippingModel(ClippingEntity clipping)
            : base(clipping)
        {
            BackingEntity = clipping;
            Content = clipping.Content;
            Device = BackingEntity.Source == ClippingDto.ClippingSourceEnum.Cloud ? Resources.FromCloud : Resources.FromLocal;
        }

        public string Content { get; private set; }

        public string Device { get; private set; }
                
        public bool IsStarred
        {
            get
            {
                return BackingEntity.IsStarred;
            }
            set
            {
                if (value.Equals(BackingEntity.IsStarred))
                {
                    return;
                }
                BackingEntity.IsStarred = value;
                NotifyOfPropertyChange(() => IsStarred);
            }
        }

        public ClippingDto.ClippingSourceEnum Source
        {
            get
            {
                return BackingEntity.Source;
            }

            set
            {
                BackingEntity.Source = value;
            }
        }
    }
}
