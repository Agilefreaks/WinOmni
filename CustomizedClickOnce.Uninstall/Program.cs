namespace CustomizedClickOnce.Uninstall
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;
    using CustomizedClickOnce.Common;
    using CustomizedClickOnce.Uninstall.Properties;
    using Omnipaste;

    public static class Program
    {
        #region Static Fields

        private static Mutex _instanceMutex;

        #endregion

        #region Public Methods and Operators

        [STAThread]
        public static void Main()
        {
            try
            {
                bool createdNew;
                _instanceMutex = new Mutex(
                    true, @"Local\" + Assembly.GetExecutingAssembly().GetType().GUID, out createdNew);
                if (createdNew)
                {
                    StartUninstallProcess();
                }
                else
                {
                    _instanceMutex = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #endregion

        #region Methods

        private static void ReleaseMutex()
        {
            if (_instanceMutex == null)
            {
                return;
            }

            _instanceMutex.ReleaseMutex();
            _instanceMutex.Close();
            _instanceMutex = null;
        }

        private static void StartUninstallProcess()
        {
            var applicationInfo = ApplicationInfoFactory.Create();
            var dialogCaption = Resources.Uninstall + applicationInfo.ProductName;
            if (MessageBox.Show(Resources.UninstallQuestion, dialogCaption, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var clickOnceHelper = new ClickOnceHelper(applicationInfo);
                clickOnceHelper.Uninstall();

                // Delete all files from publisher folder and folder itself on uninstall
                var dataFolders = new List<string>
                                      {
                                          clickOnceHelper.AppDataFolderPath,
                                          clickOnceHelper.RoamingAppDataFolderPath
                                      };
                dataFolders.Where(Directory.Exists).ToList().ForEach(folder => Directory.Delete(folder, true));
            }

            ReleaseMutex();
        }

        #endregion
    }
}