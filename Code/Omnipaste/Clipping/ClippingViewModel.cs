namespace Omnipaste.Clipping
{
    using Clipboard.Models;
    using Omnipaste.DetailsViewModel;
    using Omnipaste.Helpers;
    using OmniUI.Details;

    public class ClippingViewModel : DetailsViewModelBase<Clipping>, IClippingViewModel
    {
        #region Public Properties

        public string Content
        {
            get
            {
                return Model.Content;
            }
        }

        public Clipping.ClippingSourceEnum Source
        {
            get
            {
                return Model.Source;
            }
        }

        public bool IsLink
        {
            get
            {
                return Model.IsLink;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void OpenLink()
        {
            ExternalProcessHelper.Start(Model.Content);
        }

        #endregion
    }
}