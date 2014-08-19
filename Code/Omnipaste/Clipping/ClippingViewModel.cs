namespace Omnipaste.Clipping
{
    using System.Diagnostics;
    using Clipboard.Models;
    using Omnipaste.Event;

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
            Process.Start(Model.Content);
        }

        #endregion
    }
}