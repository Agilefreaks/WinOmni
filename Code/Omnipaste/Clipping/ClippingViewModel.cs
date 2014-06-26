namespace Omnipaste.Clipping
{
    using Caliburn.Micro;
    using Clipboard.Enums;
    using Clipboard.Models;

    public class ClippingViewModel : Screen, IClippingViewModel
    {
        #region Fields

        private Clipping _model;

        #endregion

        #region Constructors and Destructors

        public ClippingViewModel(Clipping model)
        {
            Model = model;
        }

        #endregion

        #region Public Properties

        public bool IsCloudVisible
        {
            get
            {
                return Model.Source == ClippingSourceEnum.Cloud;
            }
        }

        public bool IsLaptopVisible
        {
            get
            {
                return Model.Source == ClippingSourceEnum.Local;
            }
        }

        public Clipping Model
        {
            get
            {
                return _model;
            }
            set
            {
                _model = value;
                NotifyOfPropertyChange(() => Model);
                NotifyOfPropertyChange(() => IsCloudVisible);
                NotifyOfPropertyChange(() => IsLaptopVisible);
            }
        }

        #endregion

        public void OpenLink()
        {
            System.Diagnostics.Process.Start(Model.Content);
        }
    }
}