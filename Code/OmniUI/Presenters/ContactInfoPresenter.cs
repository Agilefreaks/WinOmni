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

        private bool _isSelected;

        #endregion

        #region Constructors and Destructors

        public ContactInfoPresenter()
        {
            ContactInfo = new ContactInfo();
        }

        public ContactInfoPresenter(IContactInfo contactInfo)
        {
            ContactInfo = contactInfo;
        }

        #endregion

        #region Public Properties

        public IContactInfo ContactInfo { get; private set; }

        public string Identifier
        {
            get
            {
                if (_identifier == null)
                {
                    UpdateContactIdentifier();
                }

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
                if (_image == null)
                {
                    UpdateContactImage();
                }

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

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (value.Equals(_isSelected))
                {
                    return;
                }
                _isSelected = value;
                NotifyOfPropertyChange();
            }
        }

        public string Name
        {
            get
            {
                return ContactInfo.Name;
            }
        }

        #endregion

        #region Methods

        protected virtual ImageSource GetContactImage()
        {
            return ContactInfo.ImageUri != null ? GetImageSourceFromUri(ContactInfo.ImageUri) : GetDefaultUserImage();
        }

        protected virtual string GetIdentifier()
        {
            return string.IsNullOrWhiteSpace(ContactInfo.Name) ? ContactInfo.Phone : ContactInfo.Name;
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

        private void UpdateContactIdentifier()
        {
            var identifier = GetIdentifier();
            _identifier = string.IsNullOrWhiteSpace(identifier) ? DefaultContactIdentifier : identifier;
            NotifyOfPropertyChange(() => Identifier);
        }

        private void UpdateContactImage()
        {
            DispatcherProvider.Application.Dispatch(
                (Action)(() =>
                    {
                        _image = GetContactImage();
                        NotifyOfPropertyChange(() => Image);
                    }));
        }

        #endregion
    }
}