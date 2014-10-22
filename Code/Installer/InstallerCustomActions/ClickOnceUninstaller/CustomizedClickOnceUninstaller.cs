namespace InstallerCustomActions.ClickOnceUninstaller
{
    using System.Collections.Generic;
    using System.Linq;
    using CustomizedClickOnce.Common;
    using InstallerCustomActions.ClickOnceMigration;

    public class CustomizedClickOnceUninstaller
    {
        public static string LastException { get; private set; }

        public static MigrationStepResultEnum Uninstall(ApplicationInfo applicationInfo)
        {
            LastException = null;
            var clickOnceHelper = new ClickOnceHelper(applicationInfo);
            var migrationSteps = new List<IMigrationTask>
                                     {
                                         new RestoreOriginalUninstallerTask(clickOnceHelper),
                                         new UninstallClickOnceTask(applicationInfo.ProductName),
                                         new RemoveShortcutFromStartupTask(clickOnceHelper),
                                     };

            return migrationSteps.Select(
                s =>
                {
                    var result = s.Execute();
                    if (s.Exception != null) LastException = s.Exception.ToString();

                    return result;
                }).FirstOrDefault(stepResult => stepResult != MigrationStepResultEnum.Success);
        }

        public static MigrationStepResultEnum Uninstall(string productName, string publisherName)
        {
            return Uninstall(new ApplicationInfo { ProductName = productName, PublisherName = publisherName });
        }
    }
}