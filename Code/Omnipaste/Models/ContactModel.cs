namespace Omnipaste.Models
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using OmniCommon.Helpers;
    using Omnipaste.Entities;
    using OmniUI.Helpers;
    using OmniUI.Models;
    using OmniUI.Properties;

    public interface IContactModel : IModel<ContactEntity>, INotifyPropertyChanged
    {
        string Identifier { get; set; }

        ImageSource Image { get; set; }

        bool IsStarred { get; set; }
    }

    public class ContactModel : Model<ContactEntity>, IContactModel
    {
        public const string UserPlaceholderBrush = "UserPlaceholderBrush";

        private static readonly string DefaultContactIdentifier = Resources.UnknownContact;

        private string _identifier;

        private ImageSource _image;

        private readonly ContactEntity _contactEntity;

        public ContactModel(ContactEntity contactEntity)
            : base(contactEntity)
        {
            _contactEntity = contactEntity;
        }
        
        public string Name
        {
            get
            {
                return BackingEntity.Name;
            }
        }

        public ContactEntity ContactEntity
        {
            get
            {
                return _contactEntity;
            }
        }

        public string Identifier
        {
            get
            {
                UpdateContactIdentifier();
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

        public bool IsStarred
        {
            get
            {
                return BackingEntity.IsStarred;
            }
            set
            {
                BackingEntity.IsStarred = value;
                NotifyOfPropertyChange(() => IsStarred);
            }
        }

        public string PhoneNumber
        {
            get
            {
                return ContactEntity.PhoneNumber;
            }

            set
            {
                if (ContactEntity.PhoneNumbers.Any())
                {
                    ContactEntity.PhoneNumbers.First().Number = value;
                }
                else
                {
                    ContactEntity.PhoneNumbers.Add(new PhoneNumber { Number = value });
                }

                NotifyOfPropertyChange(() => PhoneNumber);
                NotifyOfPropertyChange(() => Identifier);
            }
        }
        
        protected virtual ImageSource GetContactImage()
        {
            ImageSource result;

            if (!string.IsNullOrEmpty(BackingEntity.Image))
            {
                result = GetImageFromStoredData(BackingEntity.Image);
            }
            else if (BackingEntity.ImageUri != null)
            {
                result = GetImageSourceFromUri(BackingEntity.ImageUri);
            }
            else
            {
                result = GetDefaultUserImage();
            }

            return result;
        }

        protected virtual string GetIdentifier()
        {
            return string.IsNullOrWhiteSpace(BackingEntity.Name) ? BackingEntity.PhoneNumber : BackingEntity.Name;
        }

        private static BitmapImage GetImageFromStoredData(string imageData)
        {
            var imageBytes = Convert.FromBase64String(imageData);
            var memoryStream = new MemoryStream(imageBytes);

            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = memoryStream;
            image.EndInit();

            return image;
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
            
            identifier = string.IsNullOrWhiteSpace(identifier) ? DefaultContactIdentifier : identifier;
            if (_identifier != identifier)
            {
                _identifier = identifier;
                NotifyOfPropertyChange(() => Identifier);
            }
            
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
    }
}