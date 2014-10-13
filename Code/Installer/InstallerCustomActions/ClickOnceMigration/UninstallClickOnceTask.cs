namespace InstallerCustomActions.ClickOnceMigration
{
    using InstallerCustomActions.ClickOnceUninstaller;

    public class UninstallClickOnceTask : IMigrationTask
    {
        private readonly string _applicationName;

        public UninstallClickOnceTask(string applicationName)
        {
            _applicationName = applicationName;
        }

        public MigrationStepResultEnum Execute()
        {
            var result = MigrationStepResultEnum.UninstallClickOnceError;

            try
            {
                var uninstallInfo = UninstallInfo.Find(_applicationName);
                if (uninstallInfo != null)
                {
                    var uninstaller = new Uninstaller();
                    uninstaller.Uninstall(uninstallInfo);

                    result = MigrationStepResultEnum.Success;
                }
            }
            catch
            {
            }

            return result;
        }
    }
}