namespace ClickOnceTransition
{
    public enum MigrationStepResultEnum
    {
        Success = 0,

        SaveSettingsFileError,

        DownloadInstallerError,

        UninstallClickOnceError,

        RestoreSettingsFileError,

        LaunchInstallerError,

        RestoreOriginalUninstallerError
    }
}