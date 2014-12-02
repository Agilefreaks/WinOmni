﻿namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using Caliburn.Micro;
    using Omnipaste.EventAggregatorMessages;

    public class ShowConnectionTroubleshooter : PublishMessageStepBase<ShowConnectionTroubleshooterMessage>
    {
        #region Constructors and Destructors

        public ShowConnectionTroubleshooter(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
        }

        #endregion
    }
}