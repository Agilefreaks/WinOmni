namespace ClickOnceTransition
{
    using System;
    using System.IO;
    using System.Net;
    using ClickOnceTransition.Uninstaller;
    using Microsoft.Deployment.WindowsInstaller;

    public class Program
    {
        private const string InstallerFileName = "OmnipasteInstaller.msi";

        private static string _settingsFilePath;

        private static string _installerUri;

        private static string _tempFolderPath;

        private static string _installerPath;

        private static string _applicationName;

        static void Main(string[] args)
        {
            _settingsFilePath = args[1];
            _installerUri = args[3];
            _applicationName = args[5];
            _tempFolderPath = Path.GetTempPath();
            _installerPath = Path.Combine(Path.GetTempPath(), InstallerFileName);
            
            SaveSettingsFile();

            DownloadInstaller();

            UninstallClickOnceOmnipaste();

            RestoreSettingsFile();
            
            LaunchInstaller();

            Console.ReadLine();
        }

        private static void LaunchInstaller()
        {
            var installerPath = Path.Combine(Path.GetTempPath(), "OmnipasteInstaller.msi");
            Installer.SetInternalUI(InstallUIOptions.Silent);
            Installer.InstallProduct(installerPath, "");
        }

        private static void UninstallClickOnceOmnipaste()
        {
            var uninstallInfo = UninstallInfo.Find(_applicationName);
            if (uninstallInfo != null)
            {
                var uninstaller = new Uninstaller.Uninstaller();
                uninstaller.Uninstall(uninstallInfo);
            }
        }

        private static void DownloadInstaller()
        {
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(_installerUri, _installerPath);
            }
        }

        private static void SaveSettingsFile()
        {
            var sourceFilePath = _settingsFilePath;

            var destinationFileName = Path.GetFileName(sourceFilePath);
            var destinationFilePath = Path.Combine(Path.GetTempPath(), destinationFileName);
            
            File.Copy(_settingsFilePath, destinationFilePath, true);
        }

        private static void RestoreSettingsFile()
        {
            var sourceFileName = Path.GetFileName(_settingsFilePath);
            var sourceFilePath = Path.Combine(Path.GetTempPath(), sourceFileName);

            File.Copy(sourceFilePath, _settingsFilePath, true);}
    }
}
