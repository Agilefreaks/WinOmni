﻿namespace Omnipaste.Presenters
{
    using System;
    using System.IO;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using OmniUI.Helpers;
    using OmniUI.Properties;

    public class ContactInfoPresenter : Presenter<ContactInfo>, IContactInfoPresenter
    {
        public const string UserPlaceholderBrush = "UserPlaceholderBrush";

        private static readonly string DefaultContactIdentifier = Resources.UnknownContact;

        private string _identifier;

        private ImageSource _image;

        public ContactInfoPresenter(ContactInfo contactInfo)
            : base(contactInfo)
        {
        }

        public string Name
        {
            get
            {
                return BackingModel.Name;
            }
        }

        #region IContactInfoPresenter Members

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

        public bool IsStarred
        {
            get
            {
                return BackingModel.IsStarred;
            }
            set
            {
                BackingModel.IsStarred = value;
                NotifyOfPropertyChange(() => IsStarred);
            }
        }

        #endregion

        protected virtual ImageSource GetContactImage()
        {
            ImageSource result;

            if (!string.IsNullOrEmpty(BackingModel.Image))
            {
                result = GetImageFromStoredData(BackingModel.Image);
            }
            else if (BackingModel.ImageUri != null)
            {
                result = GetImageSourceFromUri(BackingModel.ImageUri);
            }
            else
            {
                result = GetDefaultUserImage();
            }

            return result;
        }

        protected virtual string GetIdentifier()
        {
            return string.IsNullOrWhiteSpace(BackingModel.Name) ? BackingModel.PhoneNumber : BackingModel.Name;
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
    }
}