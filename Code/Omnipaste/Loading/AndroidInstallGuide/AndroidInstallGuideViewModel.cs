namespace Omnipaste.Loading.AndroidInstallGuide
{
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;

    public class AndroidInstallGuideViewModel : Screen, IAndroidInstallGuideViewModel
    {
        #region Constructors and Destructors

        public AndroidInstallGuideViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        #endregion

        #region Public Properties

        public IEventAggregator EventAggregator { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Ok()
        {
            EventAggregator.PublishOnCurrentThread(new AndroidInstallationCompleteMessage());
        }

        #endregion
    }
}