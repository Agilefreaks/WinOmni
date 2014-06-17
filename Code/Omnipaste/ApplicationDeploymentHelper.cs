namespace Omnipaste
{
    using System.Deployment.Application;

    public static class ApplicationDeploymentHelper
    {
        public static bool IsClickOnceApplication
        {
            get
            {
                return !System.Diagnostics.Debugger.IsAttached && ApplicationDeployment.IsNetworkDeployed;
            }
        }
    }
}