namespace Omnipaste.Clipping
{
    using System.Diagnostics;
    using Caliburn.Micro;
    using Clipboard.Enums;
    using Clipboard.Models;

    public class ClippingViewModel : Screen, IClippingViewModel
    {
        #region Fields

        #endregion

        #region Constructors and Destructors

        public ClippingViewModel(Clipping model)
        {
            Model = model;
        }

        #endregion

        #region Public Properties

        public string Content
        {
            get
            {
                return Model.Content;
            }
        }

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

        public bool IsLink
        {
            get
            {
                return Model.IsLink;
            }
        }

        public Clipping Model { get; set; }

        #endregion

        #region Public Methods and Operators

        public void OpenLink()
        {
            Process.Start(Model.Content);
        }

        #endregion
    }
}