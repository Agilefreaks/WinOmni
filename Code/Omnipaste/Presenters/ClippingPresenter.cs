namespace Omnipaste.Presenters
{
    using Clipboard.Models;
    using Omnipaste.Models;
    using Omnipaste.Properties;

    public class ClippingPresenter : Presenter<ClippingModel>
    {
        public ClippingPresenter(ClippingModel clipping)
            : base(clipping)
        {
            BackingModel = clipping;
            Content = clipping.Content;
            Device = BackingModel.Source == Clipping.ClippingSourceEnum.Cloud ? Resources.FromCloud : Resources.FromLocal;
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

        public Clipping.ClippingSourceEnum Source
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
