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
        public static ActionResult RemoveFolder(Session session)
        {
            var result = ActionResult.Success;
            try
            {
                var folder = session.CustomActionData["FolderToRemove"];
                if (Directory.Exists(folder))
                {
                    Directory.Delete(folder, true);
                }
                else
                {
                    session.Log("No existing folder found");
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
    }
}
