namespace Omnipaste.Loading.AndroidInstallGuide
{
    using System;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Caliburn.Micro;
    using Gma.QrCodeNet.Encoding;
    using Gma.QrCodeNet.Encoding.Windows.Render;
    using OmniCommon.Helpers;
    using Omnipaste.Framework.Helpers;

    public class AndroidInstallGuideViewModel : Screen, IAndroidInstallGuideViewModel
    {
        #region Fields

        private Uri _androidInstallLink;

        private ImageSource _qrCodeImageData;

        #endregion

        #region Constructors and Destructors

        #endregion

        #region Public Properties

        public Uri AndroidInstallLink
        {
            get
            {
                return _androidInstallLink;
            }
            set
            {
                if (Equals(value, _androidInstallLink))
                {
                    return;
                }
                _androidInstallLink = value;
                NotifyOfPropertyChange();
                UpdateQRCodeImageData();
            }
        }

        public ImageSource QRCodeImageData
        {
            get
            {
                return _qrCodeImageData;
            }
            set
            {
                if (Equals(value, _qrCodeImageData))
                {
                    return;
                }
                _qrCodeImageData = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Public Methods and Operators

        public void OpenAndroidLink()
        {
            ExternalProcessHelper.Start(AndroidInstallLink.ToString());
        }

        #endregion

        #region Methods

        private void UpdateQRCodeImageData()
        {
            System.Action update = () => QRCodeImageData = RenderQRCodeImage();
            DispatcherProvider.Application.Dispatch(update);
        }

        private ImageSource RenderQRCodeImage()
        {
            var encoder = new QrEncoder(ErrorCorrectionLevel.M);
            QrCode qrCode;
            encoder.TryEncode(Convert.ToString(AndroidInstallLink), out qrCode);
            var wRenderer = new WriteableBitmapRenderer(
                new FixedModuleSize(4, QuietZoneModules.Two),
                Colors.Black,
                Colors.White);

            var imageSource = new WriteableBitmap(116, 116, 96, 96, PixelFormats.Gray8, null);
            wRenderer.Draw(imageSource, qrCode.Matrix);

            return imageSource;
        }

        #endregion
    }
}