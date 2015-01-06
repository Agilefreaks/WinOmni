namespace InstallerCustomActions
{
    using System;
    using System.IO;
    using InstallerCustomActions.ClickOnceMigration;
    using InstallerCustomActions.ClickOnceUninstaller;
    using Microsoft.Deployment.WindowsInstaller;

    public class CustomActions
    {
        [CustomAction]
        public static ActionResult UninstallClickOnce(Session session)
        {
            ActionResult result;
            try
            {
                var productName = session.CustomActionData["ProductName"];
                var uninstallInfo = UninstallInfo.Find(productName);
                if (uninstallInfo != null)
                {
                    var migrationResult = CustomizedClickOnceUninstaller.Uninstall(productName, session.CustomActionData["PublisherName"]);
                    if (migrationResult == MigrationStepResultEnum.Success)
                    {
                        result = ActionResult.Success;
                    }
                    else
                    {
                        session.Log(CustomizedClickOnceUninstaller.LastException ?? string.Empty);
                        throw new Exception("Could not migrate existing click once application. Failed with: " + migrationResult);
                    }
                                 
                }
                else
                {
                    session.Log("No existing ClickOnce installation found");
                    result = ActionResult.NotExecuted;
                }
            }
            catch (Exception exception)
            {
                session.Log(exception.ToString());
                result = ActionResult.Failure;
            }

            return result;
        }

        [CustomAction]
        public static ActionResult SetStartArguments(Session session)
        {
            var result = ActionResult.Success;
            try
            {
                var msiFileName = session["OriginalDatabase"];
                var authorizationKey = AuthorizationBootstrapper.ExtractAuthorizationKey(msiFileName);
                var arguments = string.IsNullOrWhiteSpace(authorizationKey)
                                    ? " -minimized -updated"
                                    : string.Format("-authorizationKey={0}", authorizationKey);
                session["STARTAPPARGUMENTS"] = arguments;
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
