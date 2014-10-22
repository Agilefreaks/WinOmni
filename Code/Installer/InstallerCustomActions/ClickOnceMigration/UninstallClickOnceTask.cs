namespace InstallerCustomActions.ClickOnceMigration
{
    using InstallerCustomActions.ClickOnceUninstaller;

    public class UninstallClickOnceTask : MigrationTaskBase
    {
        private readonly string _applicationName;

        public UninstallClickOnceTask(string applicationName)
            : base(MigrationStepResultEnum.UninstallClickOnceError)
        {
            _applicationName = applicationName;
        }

        protected override MigrationStepResultEnum ExecuteCore()
        {
            var uninstallInfo = UninstallInfo.Find(_applicationName);

            if (uninstallInfo == null) return MigrationStepResultEnum.UninstallClickOnceError;

            var uninstaller = new Uninstaller();
            uninstaller.Uninstall(uninstallInfo);

            return MigrationStepResultEnum.Success;
        }
    }
}