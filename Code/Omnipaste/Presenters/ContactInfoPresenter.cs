﻿namespace Omnipaste.Presenters
{
    using System;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Caliburn.Micro;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using OmniUI.Helpers;
    using Action = System.Action;

    public class ContactInfoPresenter : PropertyChangedBase, IContactInfoPresenter
    {
        #region Constants

        public const string UserPlaceholderBrush = "UserPlaceholderBrush";

        #endregion

        #region Fields

        private readonly ContactInfo _contactInfo;

        private string _identifier;

        private ImageSource _image;

        #endregion

        #region Constructors and Destructors

        public ContactInfoPresenter(ContactInfo contactInfo)
        {
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