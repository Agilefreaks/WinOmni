namespace OmniUI.Presenters
{
    using System;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Caliburn.Micro;
    using OmniCommon.Helpers;
    using OmniUI.Helpers;
    using OmniUI.Models;
    using Action = System.Action;

    public class ContactInfoPresenter : PropertyChangedBase, IContactInfoPresenter
    {
        #region Constants

        public const string UserPlaceholderBrush = "UserPlaceholderBrush";

        private const string DefaultContactIdentifier = "Unknown Contact";

        #endregion

        #region Fields

        private string _identifier;

        private ImageSource _image;

        #endregion

        #region Constructors and Destructors

        public ContactInfoPresenter()
        {
            Image = GetDefaultUserImage();
            Identifier = DefaultContactIdentifier;
        }

        public ContactInfoPresenter(IContactInfo contactInfo)
        {
            ContactInfo = contactInfo;
            UpdateContactDetails();
        }

        #endregion

        #region Public Properties

        public IContactInfo ContactInfo { get; private set; }

        public string Identifier
        {
            get
            {
                return _identifier;
            }
            set
            {
                if (value == _identifier)
                {
                    return;
                }
                _identifier = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageSource Image
        {
            get
            {
                return _image;
            }
            set
            {
                if (Equals(value, _image))
                {
                    return;
                }
                _image = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Methods

        protected virtual void UpdateContactIdentifier()
        {
            Identifier = string.IsNullOrWhiteSpace(ContactInfo.Name) ? ContactInfo.Phone : ContactInfo.Name;
        }

        private static ImageSource GetDefaultUserImage()
        {
            var resource = ResourceHelper.GetByKey(UserPlaceholderBrush);
            return new DrawingImage(((DrawingBrush)resource).Drawing);
        }

        private static ImageSource GetImageSourceFromUri(Uri uri)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = uri;
            image.EndInit();

            return image;
        }

        private void UpdateContactDetails()
        {
            UpdateContactImage();
            UpdateContactIdentifier();
        }

        private void UpdateContactImage()
        {
            DispatcherProvider.Application.Dispatch(
                (Action)
                (() =>
                    {
                        Image = ContactInfo.ImageUri != null
                                    ? GetImageSourceFromUri(ContactInfo.ImageUri)
                                    : GetDefaultUserImage();
                    }));
        }

        #endregion
    }
}