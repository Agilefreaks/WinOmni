namespace InstallerCustomActions.ClickOnceMigration
{
    public enum MigrationStepResultEnum
    {
        Success = 0,

        UninstallClickOnceError,

        RestoreOriginalUninstallerError
    }
}