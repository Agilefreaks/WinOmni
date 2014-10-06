namespace ClickOnceTransition
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using ClickOnceTransition.Uninstaller;
    using CustomizedClickOnce.Common;

    public class ClickOneTransition
    {
        private const string InstallerFileName = "OmnipasteInstaller.msi";

        private static string _installerUri;

        private static string _tempFolderPath;

        private static string _installerPath;

        private static string _applicationName;

        protected static ClickOnceHelper ClickOnceHelper
        {
            get
            {
                return
                    new ClickOnceHelper(
                        new ApplicationInfo { ProductName = _applicationName, PublisherName = "Omnipaste" });
            }
        }

        static int Main(string[] args)
        {
            _installerUri = args[1];
            _applicationName = args[3];
            _tempFolderPath = Path.GetTempPath();
            _installerPath = Path.Combine(_tempFolderPath, InstallerFileName);

            var migrationSteps = SetupMigrationSteps();

            var migrationResult = migrationSteps.Select(s => s()).FirstOrDefault(stepResult => stepResult != MigrationStepResultEnum.Success);

#if DEBUG
            Console.ReadLine();
#endif
            switch (migrationResult)
            {
                case MigrationStepResultEnum.UninstallClickOnceError:
                case MigrationStepResultEnum.InstallNewVersionError:
                    ShowUpdateFailedWebPage();
                    break;
            }

            return (int)migrationResult;
        }

        private static IEnumerable<Func<MigrationStepResultEnum>> SetupMigrationSteps()
        {
            var migrationSteps = new List<Func<MigrationStepResultEnum>>
                                 {
                                     DownloadInstaller,
                                     RestoreOriginalUninstaller,
                                     CloseRunningInstance,
                                     UninstallClickOnceOmnipaste,
                                     RemoveShortcutFromStartup,
                                     InstallNewVersion
                                 };

            return migrationSteps;
        }

        private static MigrationStepResultEnum RemoveShortcutFromStartup()
        {
            try
            {
                ClickOnceHelper.RemoveShortcutFromStartup();
            }
            catch
            {
            }

            return MigrationStepResultEnum.Success;
        }

        private static MigrationStepResultEnum RestoreOriginalUninstaller()
        {
            var result = MigrationStepResultEnum.RestoreOriginalUninstallerError;

            try
            {
                if (ClickOnceHelper.RestoreOriginalUninstaller())
                {
                    result = MigrationStepResultEnum.Success;
                }
            }
            catch
            {
            }

            return result;
        }

        private static MigrationStepResultEnum InstallNewVersion()
        {
            var result = MigrationStepResultEnum.InstallNewVersionError;

            try
            {
                Process.Start(
                    new ProcessStartInfo
                        {
                            FileName = "msiexec.exe",
                            Arguments = string.Format("/package {0} /quiet", _installerPath)
                        });
                result = MigrationStepResultEnum.Success;
            }
            catch
            {
            }

            return result;
        }

        private static MigrationStepResultEnum UninstallClickOnceOmnipaste()
        {
            var result = MigrationStepResultEnum.UninstallClickOnceError;

            try
            {
                var uninstallInfo = UninstallInfo.Find(_applicationName);
                if (uninstallInfo != null)
                {
                    var uninstaller = new Uninstaller.Uninstaller();
                    uninstaller.Uninstall(uninstallInfo);

                    result = MigrationStepResultEnum.Success;
                }
            }
            catch
            {
            }

            return result;
        }

        private static MigrationStepResultEnum DownloadInstaller()
        {
            var result = MigrationStepResultEnum.DownloadInstallerError;
            using (var webClient = new WebClient())
            {
                try
                {
                    webClient.DownloadFile(_installerUri, _installerPath);
                    result = MigrationStepResultEnum.Success;
                }
                catch (Exception)
                {
                }
            }

            return result;
        }

        private static void ShowUpdateFailedWebPage()
        {
            Process.Start("https://www.omnipasteapp.com/downloads/new?download=true&migration_error=true");
        }

        private static MigrationStepResultEnum CloseRunningInstance()
        {
            try
            {
                ClickOnceHelper.KillActiveProcesses();
            }
            catch
            {
            }

            return MigrationStepResultEnum.Success;
        }
    }
}
