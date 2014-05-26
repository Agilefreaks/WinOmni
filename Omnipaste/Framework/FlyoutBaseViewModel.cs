using Caliburn.Micro;
using MahApps.Metro.Controls;
using Omnipaste.UserToken;

namespace Omnipaste.Framework
{
    public abstract class FlyoutBaseViewModel : Screen, IFlyoutViewModel
    {
        #region private fields
        private string _header;

        private bool _isOpen;

        private Position _position;

        private bool _isModal;
        #endregion

        public string Header
        {
            get
            {
                return _header;
            }
            set
            {
                if (_header != value)
                {
                    _header = value;
                    NotifyOfPropertyChange(() => Header);
                }
            }
        }

        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                if (_isOpen != value)
                {
                    _isOpen = value;
                    NotifyOfPropertyChange(() => IsOpen);
                }
            }
        }

        public bool IsModal
        {
            get
            {
                return _isModal;
            }
            set
            {
                if (_isModal != value)
                {
                    _isModal = value;
                    NotifyOfPropertyChange(() => IsModal);
                }
            }
        }

        public Position Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    NotifyOfPropertyChange(() => Position);
                }
                
            }
        }
    }
}