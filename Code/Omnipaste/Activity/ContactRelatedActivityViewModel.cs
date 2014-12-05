namespace Omnipaste.Activity
{
    using System;
    using System.Reflection;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using OmniCommon.Helpers;
    using OmniUI.Attributes;

    [UseView("Omnipaste.Activity.ActivityView", IsFullyQualifiedName = true)]
    public class ContactRelatedActivityViewModel : ActivityViewModel, IContactRelatedActivityViewModel
    {
        #region Constants

        private const string DefaultContactImagePathFormat = @"/{0};component/Resources/DefaultUserIcon.png";

        #endregion

        #region Fields

        private readonly Uri _defaultContactImageUri;

        private string _contactIdentifier;

        private ImageSource _contactImage;

        #endregion

        #region Constructors and Destructors

        public ContactRelatedActivityViewModel()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            _defaultContactImageUri = new Uri(
                string.Format(DefaultContactImagePathFormat, assemblyName),
                UriKind.Relative);
        }

        #endregion

        #region Public Properties

        public string ContactIdentifier
        {
            get
            {
                return _contactIdentifier;
            }
            set
            {
                if (value == _contactIdentifier)
                {
                    return;
                }
                _contactIdentifier = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageSource ContactImage
        {
            get
            {
                return _contactImage;
            }
            set
            {
                if (Equals(value, _contactImage))
                {
                    return;
                }
                _contactImage = value;
                NotifyOfPropertyChange();
            }
        }

        public override Models.Activity Model
        {
            get
            {
                return base.Model;
            }
            set
            {
                base.Model = value;
                UpdateContactImage();
                UpdateContactIdentifier();
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

        private void UpdateContactIdentifier()
        {
            string identifier = null;
            if (Model.ContactInfo != null)
            {
                identifier = string.IsNullOrWhiteSpace(Model.ContactInfo.Name)
                                 ? Model.ContactInfo.Phone
                                 : Model.ContactInfo.Name;
            }

            ContactIdentifier = identifier;
        }

        private void UpdateContactImage()
        {
            DispatcherProvider.Application.Dispatch(
                (Action)
                (() =>
                    {
                        ContactImage = Model.ContactInfo.ImageUri != null
                                           ? GetImageSourceFromUri(Model.ContactInfo.ImageUri)
                                           : GetDefaultUserImage();
                    }));
        }

        #endregion
    }
}