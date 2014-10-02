namespace CustomizedClickOnce.Common
{
    using System;
    using System.Collections.Generic;
    using System.Deployment.Application;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.AccessControl;
    using System.Threading;
    using Microsoft.Win32;

    public class ClickOnceHelper : IClickOnceHelper
    {
        #region Constants

        private const string ApprefExtension = ".appref-ms";

        private const string DisplayNameKey = "DisplayName";

        private const string UninstallString = "UninstallString";

        private const string UninstallStringFile = "UninstallString.bat";

        private const string UninstallerName = "uninstall.exe";

        #endregion

        #region Static Fields

        private static string _location;

        private static string _uninstallerPath;

        #endregion

        #region Fields

        private readonly ApplicationInfo _applicationInfo;

        private readonly RegistryKey _uninstallRegistryKey;

        private string _appDataFolder;

        private string _roamingAppDataFolder;

        private string _shortcutPath;

        private string _startupShortcutPath;

        private string _uninstallFilePath;

        #endregion

        #region Constructors and Destructors

        public ClickOnceHelper(ApplicationInfo applicationInfo)
        {
            _applicationInfo = applicationInfo;
            _uninstallRegistryKey = GetUninstallRegistryKeyByProductName(ProductName);
        }

        #endregion

        #region Public Properties

        public string AppDataFolderPath
        {
            get
            {
                return _appDataFolder ?? (_appDataFolder = GetAppDataFolderPath());
            }
        }

        public string ProductName
        {
            get
            {
                return _applicationInfo.ProductName;
            }
        }

        public string PublisherName
        {
            get
            {
                return _applicationInfo.PublisherName;
            }
        }

        public string RoamingAppDataFolderPath
        {
            get
            {
                return _roamingAppDataFolder ?? (_roamingAppDataFolder = GetRoamingAppDataFolderPath());
            }
        }

        #endregion

        #region Properties

        private static string Location
        {
            get
            {
                return _location ?? (_location = Assembly.GetExecutingAssembly().Location);
            }
        }

        private static string UninstallerPath
        {
            get
            {
                return _uninstallerPath ?? (_uninstallerPath = GetUninstallerPath());
            }
        }

        private string ShortcutPath
        {
            get
            {
                return _shortcutPath ?? (_shortcutPath = GetShortcutPath());
            }
        }

        private string StartupShortcutPath
        {
            get
            {
                return _startupShortcutPath ?? (_startupShortcutPath = GetStartupShortcutPath());
            }
        }

        private string UninstallFilePath
        {
            get
            {
                return _uninstallFilePath ?? (_uninstallFilePath = GetUninstallFilePath());
            }
        }

        #endregion

        #region Public Methods and Operators

        public void AddShortcutToStartup()
        {
            if (!ApplicationDeployment.IsNetworkDeployed || File.Exists(StartupShortcutPath))
            {
                return;
            }

            File.Copy(ShortcutPath, StartupShortcutPath);
        }

        public void KillActiveProcesses()
        {
            foreach (Process process in Process.GetProcessesByName(ProductName))
            {
                process.Kill();
                break;
            }
        }

        public void RemoveShortcutFromStartup()
        {
            if (File.Exists(StartupShortcutPath))
            {
                File.Delete(StartupShortcutPath);
            }
        }

        public bool RestoreOriginalUninstaller()
        {
            bool result = false;

            if (File.Exists(UninstallFilePath))
            {
                string uninstallString = File.ReadAllText(UninstallFilePath);
                _uninstallRegistryKey.SetValue(UninstallString, uninstallString);

                result = true;
            }

            return result;
        }

        public bool StartupShortcutExists()
        {
            return File.Exists(StartupShortcutPath);
        }

        public void Uninstall()
        {
            if (!File.Exists(UninstallFilePath))
            {
                return;
            }

            try
            {
                KillActiveProcesses();
                Process uninstallProcess = RunOriginalUninstaller();
                WaitForProcessToFinish(uninstallProcess);

                if (ApplicationIsUninstalled())
                {
                    RemoveShortcutFromStartup();
                    RemoveDataFolders();
                }
                else
                {
                    UpdateUninstallParameters();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public void UpdateUninstallParameters()
        {
            if (_uninstallRegistryKey == null)
            {
                return;
            }

            AssureAppDataFolderExists();
            UpdateUninstallString();
            UpdateDisplayIcon();
            SetNoModify();
            SetNoRepair();
            SetHelpLink();
            SetUrlInfoAbout();
        }

        #endregion

        #region Methods

        private static IEnumerable<RegistryKey> GetAccessibleUninstallKeys(RegistryKey uninstallKey)
        {
            return
                uninstallKey.GetSubKeyNames()
                    .Select(
                        subKeyName =>
                            uninstallKey.OpenSubKey(
                                subKeyName,
                                RegistryKeyPermissionCheck.ReadWriteSubTree,
                                RegistryRights.QueryValues | RegistryRights.ReadKey | RegistryRights.WriteKey))
                    .Where(subKey => subKey != null);
        }

        private static RegistryKey GetUninstallRegistryKeyByProductName(string productName)
        {
            RegistryKey uninstallKey =
                Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");
            if (uninstallKey == null)
            {
                return null;
            }

            IEnumerable<RegistryKey> accessibleUninstallKeys = GetAccessibleUninstallKeys(uninstallKey);
            IEnumerable<RegistryKey> uninstallKeyQuery = from uninstallKeys in accessibleUninstallKeys
                from valueKey in uninstallKeys.GetValueNames().Where(valueKey => valueKey.Equals(DisplayNameKey))
                where uninstallKeys.GetValue(valueKey).Equals(productName)
                select uninstallKeys;

            return uninstallKeyQuery.FirstOrDefault();
        }

        private static string GetUninstallerPath()
        {
            string directoryName = Path.GetDirectoryName(Location);
            Debug.Assert(directoryName != null, "Should be able to get the folder where this assembly resides.");

            return Path.Combine(directoryName, UninstallerName);
        }

        private static void WaitForProcessToFinish(Process uninstallProcess)
        {
            while (!uninstallProcess.HasExited)
            {
                Thread.Sleep(100);
                uninstallProcess.Refresh();
            }
        }

        private bool ApplicationIsUninstalled()
        {
            return GetUninstallRegistryKeyByProductName(ProductName) == null;
        }

        private void AssureAppDataFolderExists()
        {
            if (!Directory.Exists(AppDataFolderPath))
            {
                Directory.CreateDirectory(AppDataFolderPath);
            }
        }

        private string GetAppDataFolderPath()
        {
            return
                Path.Combine(
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        PublisherName),
                    ProductName);
        }

        private string GetRoamingAppDataFolderPath()
        {
            return
                Path.Combine(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PublisherName),
                    ProductName);
        }

        private string GetShortcutPath()
        {
            string allProgramsPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
            string shortcutPath = Path.Combine(Path.Combine(allProgramsPath, PublisherName), ProductName);

            return Path.Combine(shortcutPath, ProductName + ApprefExtension);
        }

        private string GetStartupShortcutPath()
        {
            string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            return Path.Combine(startupPath, ProductName + ApprefExtension);
        }

        private string GetUninstallFilePath()
        {
            return Path.Combine(AppDataFolderPath, UninstallStringFile);
        }

        private void RemoveDataFolders()
        {
            var dataFolders = new List<string> { AppDataFolderPath, RoamingAppDataFolderPath };
            dataFolders.Where(Directory.Exists).ToList().ForEach(folder => Directory.Delete(folder, true));
        }

        private Process RunOriginalUninstaller()
        {
            string uninstallString = File.ReadAllText(UninstallFilePath);
            string fileName = uninstallString.Substring(0, uninstallString.IndexOf(" ", StringComparison.Ordinal));
            string args = uninstallString.Substring(uninstallString.IndexOf(" ", StringComparison.Ordinal) + 1);

            var uninstallProcess = new Process
                                   {
                                       StartInfo =
                                       {
                                           Arguments = args,
                                           FileName = fileName,
                                           UseShellExecute = false
                                       }
                                   };

            uninstallProcess.Start();
            return uninstallProcess;
        }

        private void SetHelpLink()
        {
            _uninstallRegistryKey.SetValue("HelpLink", _applicationInfo.HelpLink, RegistryValueKind.String);
        }

        private void SetNoModify()
        {
            _uninstallRegistryKey.SetValue("NoModify", 1, RegistryValueKind.DWord);
        }

        private void SetNoRepair()
        {
            _uninstallRegistryKey.SetValue("NoRepair", 1, RegistryValueKind.DWord);
        }

        private void SetUrlInfoAbout()
        {
            _uninstallRegistryKey.SetValue("URLInfoAbout", _applicationInfo.AboutLink, RegistryValueKind.String);
        }

        private void UpdateDisplayIcon()
        {
            string str = string.Format("{0},0", UninstallerPath);
            _uninstallRegistryKey.SetValue("DisplayIcon", str);
        }

        private void UpdateUninstallString()
        {
            var uninstallString = (string)_uninstallRegistryKey.GetValue(UninstallString);
            if (!string.IsNullOrEmpty(UninstallFilePath) && uninstallString.StartsWith("rundll32.exe"))
            {
                File.WriteAllText(UninstallFilePath, uninstallString);
            }

            string str = string.Format("\"{0}\" uninstall", UninstallerPath);
            _uninstallRegistryKey.SetValue(UninstallString, str);
        }

        #endregion
    }
}