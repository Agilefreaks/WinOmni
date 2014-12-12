namespace Omnipaste.Activity.Presenters
{
    using System;
    using System.Reflection;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using Caliburn.Micro;

    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using Action = System.Action;

    public class ContactInfoPresenter : PropertyChangedBase, IContactInfoPresenter
    {
        #region Constants

        private const string DefaultContactImagePathFormat = @"/{0};component/Resources/DefaultUserIcon.png";

        #endregion

        #region Fields

        private readonly ContactInfo _contactInfo;

        private readonly Uri _defaultContactImageUri;

        private string _identifier;

        private ImageSource _image;

        #endregion

        #region Constructors and Destructors

        public ContactInfoPresenter(ContactInfo contactInfo)
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            _defaultContactImageUri = new Uri(
                string.Format(DefaultContactImagePathFormat, assemblyName),
                UriKind.Relative);
            _contactInfo = contactInfo;
            UpdateContactDetails();
        }

        #endregion

        #region Public Properties

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

        private static ImageSource GetImageSourceFromUri(Uri uri)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = uri;
            image.EndInit();

            return image;
        }

        private ImageSource GetDefaultUserImage()
        {
            return GetImageSourceFromUri(_defaultContactImageUri);
        }

        private void UpdateContactDetails()
        {
            UpdateContactImage();
            UpdateContactIdentifier();
        }

        private void UpdateContactIdentifier()
        {
            Identifier = string.IsNullOrWhiteSpace(_contactInfo.Name) ? _contactInfo.Phone : _contactInfo.Name;
        }

        private void UpdateContactImage()
        {
            DispatcherProvider.Application.Dispatch(
                (Action)
                (() =>
                    {
                        Image = _contactInfo.ImageUri != null
                                           ? GetImageSourceFromUri(_contactInfo.ImageUri)
                                           : GetDefaultUserImage();
                    }));
        }

        #endregion
    }
}