﻿namespace Omnipaste.Services.ActivationServiceData.ActivationServiceSteps
{
    using System;
    using Caliburn.Micro;
    using Omnipaste.EventAggregatorMessages;

    public class AndroidInstallGuide : SynchronousStepBase
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        #endregion

        #region Constructors and Destructors

        public AndroidInstallGuide(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        #endregion

        #region Methods

        protected override IExecuteResult ExecuteSynchronously()
        {
            _eventAggregator.PublishOnUIThread(
                new ShowAndroidInstallGuideMessage { AndroidInstallLink = (Uri)Parameter.Value });
            return new ExecuteResult(SimpleStepStateEnum.Successful);
        }

        #endregion
    }
}