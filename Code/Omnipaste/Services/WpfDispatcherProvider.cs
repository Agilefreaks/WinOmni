﻿namespace Omnipaste.Services
{
    using OmniCommon.Helpers;
    using OmniUI.Helpers;

    public class WpfDispatcherProvider : IDispatcherProvider
    {
        public IDispatcher Current
        {
            get
            {
                return DispatcherWrapper.FromCurrent();
            }
        }

        public IDispatcher Application
        {
            get
            {
                return DispatcherWrapper.FromGiven(ApplicationHelper.Instance.Dispatcher);
            }
        }
    }
}