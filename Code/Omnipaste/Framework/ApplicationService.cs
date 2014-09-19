namespace Omnipaste.Framework
{
    using System;
    using System.Deployment.Application;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Threading;
    using OmniCommon.Interfaces;

    public class ApplicationService : IApplicationService
    {
        #region Public Properties

        public Dispatcher Dispatcher
        {
            get
            {
                return Application.Current.Dispatcher;
            }
        }

        public Version Version
        {
            get
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;

                if (ApplicationDeploymentHelper.IsClickOnceApplication)
                {
                    ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                    version = ad.CurrentVersion;
                }

                return version;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void ShutDown()
        {
            Dispatcher.InvokeShutdown();
        }

        #endregion
    }
}