namespace CustomizedClickOnce.Common
{
    public interface IClickOnceHelper
    {
        #region Public Properties

        string ProductName { get; }

        string AppDataFolderPath { get; }

        string PublisherName { get; }

        #endregion

        #region Public Methods and Operators

        void AddShortcutToStartup();

        void RemoveShortcutFromStartup();

        void Uninstall();

        void UpdateUninstallParameters();

        bool StartupShortcutExists();

        #endregion
    }
}