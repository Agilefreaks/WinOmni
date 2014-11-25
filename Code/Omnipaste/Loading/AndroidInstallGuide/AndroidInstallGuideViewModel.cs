namespace Omnipaste.Loading.AndroidInstallGuide
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using Caliburn.Micro;

    public class AndroidInstallGuideViewModel : Screen, IAndroidInstallGuideViewModel
    {
        private Uri _androidInstallLink;

        #region Constructors and Destructors

        public AndroidInstallGuideViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

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
            }
        }

        public IEventAggregator EventAggregator { get; set; }

        #endregion

        #region Public Methods and Operators

        public void OpenAndroidLink()
        {
            try
            {
                Process.Start(AndroidInstallLink.ToString());
            }
            catch (Win32Exception)
            {
                // Looks like there is no way for us to act on this
            }
        }

        #endregion
    }
}