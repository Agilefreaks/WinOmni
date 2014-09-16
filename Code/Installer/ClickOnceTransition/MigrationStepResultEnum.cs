namespace ClickOnceTransition
{
    public enum MigrationStepResultEnum
    {
        Success = 0,

        DownloadInstallerError,

        UninstallClickOnceError,

        InstallNewVersionError,

        RestoreOriginalUninstallerError
    }
}