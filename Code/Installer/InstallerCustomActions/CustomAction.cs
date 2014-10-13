using Microsoft.Deployment.WindowsInstaller;

namespace InstallerCustomActions
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text.RegularExpressions;

    public class CustomActions
    {
        [CustomAction]
        public static ActionResult StartAppWithAuthorizationKey(Session session)
        {
            var result = ActionResult.Success;
            try
            {
                var targetDir = session.CustomActionData["TargetDir"];
                var target = session.CustomActionData["Target"];
                var msiFileName = session.CustomActionData["OriginalDatabase"];
                AuthorizationBootstrapper.StartApp(Path.Combine(targetDir, target), msiFileName);
            }
            catch (Exception exception)
            {
                session.Log(exception.ToString());
                result = ActionResult.Failure;
            }

            return result;
        }
    }
}
