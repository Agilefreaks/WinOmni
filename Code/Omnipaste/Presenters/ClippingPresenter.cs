namespace Omnipaste.Presenters
{
    using Clipboard.Dto;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.Properties;
    using OmniUI.Presenters;

    public class ClippingPresenter : Presenter<ClippingEntity>
    {
        public ClippingPresenter(ClippingEntity clipping)
            : base(clipping)
        {
            BackingModel = clipping;
            Content = clipping.Content;
            Device = BackingModel.Source == ClippingDto.ClippingSourceEnum.Cloud ? Resources.FromCloud : Resources.FromLocal;
        }

        public string Content { get; private set; }

        public string Device { get; private set; }
                
        public bool IsStarred
        {
            get
            {
                return BackingModel.IsStarred;
            }
            set
            {
                if (value.Equals(BackingModel.IsStarred))
                {
                    return;
                }
                BackingModel.IsStarred = value;
                NotifyOfPropertyChange(() => IsStarred);
            }
        }

        public ClippingDto.ClippingSourceEnum Source
        {
            get
            {
                return BackingModel.Source;
            }

            set
            {
                BackingModel.Source = value;
            }
        }
    }
}
