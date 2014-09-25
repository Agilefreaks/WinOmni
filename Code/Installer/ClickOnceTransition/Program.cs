namespace ClickOnceTransition
{
    using System;
    using System.IO;
    using System.Net;
    using ClickOnceTransition.Uninstaller;
    using Microsoft.Deployment.WindowsInstaller;

    class Program
    {
        static void Main(string[] args)
        {
            var settingsFilePath = args[1];
            var installerUri = args[3];
            
            SaveSettingsFile(settingsFilePath);

            DownloadInstaller(installerUri);

            UninstallClickOnceOmnipaste();

            RestoreSettingsFile(settingsFilePath);
            
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
            var uninstallInfo = UninstallInfo.Find("Omnipaste");
            if (uninstallInfo != null)
            {
                var uninstaller = new Uninstaller.Uninstaller();
                uninstaller.Uninstall(uninstallInfo);
            }
        }

        private static void DownloadInstaller(string installerUri)
        {
            var installerPath = Path.Combine(Path.GetTempPath(), "OmnipasteInstaller.msi");
        
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(installerUri, installerPath);
            }
        }

        private static void SaveSettingsFile(string settingsFilePath)
        {
            var sourceFilePath = settingsFilePath;

            var destinationFileName = Path.GetFileName(sourceFilePath);
            var destinationFilePath = Path.Combine(Path.GetTempPath(), destinationFileName);
            
            File.Copy(settingsFilePath, destinationFilePath, true);
        }

        private static void RestoreSettingsFile(string settingsFilePath)
        {
            var sourceFileName = Path.GetFileName(settingsFilePath);
            var sourceFilePath = Path.Combine(Path.GetTempPath(), sourceFileName);

            File.Copy(sourceFilePath, settingsFilePath, true);
        }
    }
}
