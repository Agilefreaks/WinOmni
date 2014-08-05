namespace Omnipaste.Loading.AndroidInstallGuide
{
    using Caliburn.Micro;
    using OmniCommon.EventAggregatorMessages;

    public class AndroidInstallGuideViewModel : Screen, IAndroidInstallGuideViewModel
    {
        public IEventAggregator EventAggregator { get; set; }

        public AndroidInstallGuideViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        public void Ok()
        {
            EventAggregator.PublishOnCurrentThread(new AndroidInstallationCompleteMessage());
        }
    }
}