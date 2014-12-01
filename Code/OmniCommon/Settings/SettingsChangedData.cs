namespace OmniCommon.Settings
{
    public class SettingsChangedData
    {
        #region Constructors and Destructors

        public SettingsChangedData()
        {
        }

        public SettingsChangedData(string settingName)
        {
            SettingName = settingName;
        }

        public SettingsChangedData(string settingName, object newValue)
            : this(settingName)
        {
            NewValue = newValue;
        }

        #endregion

        #region Public Properties

        public object NewValue { get; set; }

        public string SettingName { get; set; }

        #endregion
    }
}