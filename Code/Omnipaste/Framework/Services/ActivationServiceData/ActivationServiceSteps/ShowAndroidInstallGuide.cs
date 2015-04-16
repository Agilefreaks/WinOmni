namespace Omnipaste.Framework.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using Caliburn.Micro;
    using Omnipaste.Framework.EventAggregatorMessages;

    public class ShowAndroidInstallGuide : PublishMessageStepBase<ShowAndroidInstallGuideMessage>
    {
        #region Constructors and Destructors

        public ShowAndroidInstallGuide(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
        }

        #endregion

        #region Methods

        protected override ShowAndroidInstallGuideMessage GetMessageToSend()
        {
            return new ShowAndroidInstallGuideMessage { AndroidInstallLink = (Uri)Parameter.Value };
        }

        #endregion
    }
}